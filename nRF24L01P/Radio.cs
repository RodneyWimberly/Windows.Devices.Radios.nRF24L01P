using System;
using System.Diagnostics;
using Windows.Devices.Gpio;
using Windows.Devices.Radios.nRF24L01P.Enums;
using Windows.Devices.Radios.nRF24L01P.Registers;
using Windows.Devices.Spi;

namespace Windows.Devices.Radios.nRF24L01P
{
    /// <summary>
    /// Driver for nRF24L01(+) 2.4GHz Wireless Transceiver
    /// </summary>
    public class Radio
    {
        private long _receiveAddressPipe0;
        private readonly object _spiLock;
        private readonly GpioPin _cePin;
        private bool _checkStatus;

        public ICommandProcessor CommandProcessor { get; private set; }

        public RadioConfiguration Configuration { get; private set; }

        public TransmitPipe TransmitPipe { get; private set; }

        public ReceivePipeCollection ReceivePipes { get; private set; }

        private bool _isAckPayloadAvailable;
        public bool IsAckPayloadAvailable
        {
            get
            {
                bool isAckPayloadAvailable = _isAckPayloadAvailable;
                _isAckPayloadAvailable = false;
                return isAckPayloadAvailable;
            }
        }

        public string Name => Constants.RadioModels[(int)Configuration.RadioModel];

        public bool IsDataAvailable => Configuration.Registers.StatusRegister.RX_DR;

        public Radio(GpioPin cePin, SpiDevice spiDevice)
        {
            _checkStatus = false;
            _isAckPayloadAvailable = false;
            _receiveAddressPipe0 = 0;
            _spiLock = new object();

            _cePin = cePin;
            _cePin.SetDriveMode(GpioPinDriveMode.Output);

            CommandProcessor = new CommandProcessor(spiDevice, this);
            Configuration = new RadioConfiguration(CommandProcessor);
            CommandProcessor.StatusRegisterLoad = Configuration.Registers.StatusRegister.Load;
            TransmitPipe = new TransmitPipe(Configuration, CommandProcessor);
            ReceivePipes = new ReceivePipeCollection(Configuration, CommandProcessor);
        }

        public string GetDetails()
        {
            Diagnostics diagnostics = new Diagnostics(this);
            return diagnostics.GetDetails();
        }

        public void ChipEnable(bool enabled)
        {
            _cePin.Write(enabled ? GpioPinValue.High : GpioPinValue.Low);

            // Must allow the radio time to settle else configuration bits will not necessarily stick.
            // This is actually only required following power up but some settling time also appears to
            // be required after resets too. For full coverage, we'll always assume the worst.
            // Enabling 16b CRC is by far the most obvious case if the wrong timing is used - or skipped.
            // Technically we require 4.5ms + 14us as a worst case. We'll just call it 5ms for good measure.
            // WARNING: Delay is based on P-variant whereby non-P *may* require different timing.
            Utilities.DelayMicroseconds(5000);
        }

        protected byte WritePayload(byte[] payload)
        {
            int payloadSize = Math.Min(payload.Length, Configuration.PayloadWidth);
            int blankLength = Configuration.DynamicPayloadLengthEnabled ? 0 : Configuration.PayloadWidth - payloadSize;
            if (blankLength > 0)
            {
                int originalLength = payload.Length;
                Array.Resize(ref payload, originalLength + blankLength);
                for (int index = 0; index < blankLength; index++)
                    payload[originalLength + index] = 0;
            }

            ChipEnable(false);
            CommandProcessor.ExecuteCommand(DeviceCommands.W_TX_PAYLOAD, RegisterAddresses.EMPTY_ADDRESS, payload);
            ChipEnable(true);

            return Configuration.Registers.StatusRegister;
        }

        protected byte ReadPayload(ref byte[] payload, int length)
        {
            int payloadSize = Math.Min(length, Configuration.PayloadWidth);
            int blankLength = Configuration.DynamicPayloadLengthEnabled ? 0 : Configuration.PayloadWidth - payloadSize;

            byte[] request = new byte[payloadSize + blankLength];
            for (int index = 0; index < payloadSize; index++)
                request[index] = (byte)DeviceCommands.NOP;

            ChipEnable(false);
            payload = CommandProcessor.ExecuteCommand(DeviceCommands.R_RX_PAYLOAD, RegisterAddresses.EMPTY_ADDRESS, request);
            ChipEnable(true);

            return Configuration.Registers.StatusRegister;
        }

        public void Begin()
        {
            RegisterManager registers = Configuration.Registers;

            ChipEnable(false);

            // Set 1500uS (minimum for 32B payload in ESB@250KBPS) timeouts, to make testing a little easier
            // WARNING: If this is ever lowered, either 250KBS mode with AA is broken or maximum packet
            // sizes must never be used. See documentation for a more complete explanation.
            Configuration.AutoRetransmitDelay = AutoRetransmitDelays.Delay1500uS;
            Configuration.AutoRetransmitCount = 15;

            // Restore our default PA level
            Configuration.PowerLevel = PowerLevels.Max;

            // Attempt to set DataRate to 250Kbps to determine if this is a plus model
            Configuration.DataRate = DataRates.DataRate250Kbps;
            Configuration.IsPlusModel = Configuration.DataRate == DataRates.DataRate250Kbps;

            // Then set the data rate to the slowest (and most reliable) speed supported by all hardware.
            Configuration.DataRate = DataRates.DataRate1Mbps;

            // Initialize CRC and request 2-byte (16bit) CRC
            Configuration.CrcEncodingScheme = CrcEncodingSchemes.DualBytes;
            Configuration.CrcEnabled = true;

            // Disable auto acknowledgement 
            EnableAutoAcknowledgementRegister autoAckRegister = registers.EnableAutoAcknowledgementRegister;
            autoAckRegister.EN_AA = false;
            autoAckRegister.Save();

            // Disable dynamic payload lengths
            Configuration.DynamicPayloadLengthEnabled = false;

            // Reset current status
            // Notice reset and flush is the last thing we do
            StatusRegister statusRegister = registers.StatusRegister;
            statusRegister.RX_DR = false;
            statusRegister.TX_DS = false;
            statusRegister.MAX_RT = false;
            statusRegister.Save();

            // Set up default configuration.  Callers can always change it later.
            // This channel should be universally safe and not bleed over into adjacent spectrum.
            Configuration.Channel = 76;

            ReceivePipes.FlushBuffer();
            TransmitPipe.FlushBuffer();
        }

        public void StartListening()
        {
            ConfigurationRegister configRegister = Configuration.Registers.ConfigurationRegister;
            configRegister.PWR_UP = true;
            configRegister.PRIM_RX = true;
            configRegister.Save();

            StatusRegister statusRegister = Configuration.Registers.StatusRegister;
            statusRegister.RX_DR = false;
            statusRegister.TX_DS = false;
            statusRegister.MAX_RT = false;
            statusRegister.Save();

            // Restore the pipe0 address, if exists
            if (_receiveAddressPipe0 > 0)
            {
                AddressPipeRegister receiveAddressPipe0Register = Configuration.Registers.ReceiveAddressPipeRegisters[0];
                receiveAddressPipe0Register.Load(BitConverter.GetBytes(_receiveAddressPipe0));
                receiveAddressPipe0Register.Save();
            }

            TransmitPipe.FlushBuffer();
            ReceivePipes.FlushBuffer();

            ChipEnable(true);
        }

        public void StopListening()
        {
            ChipEnable(false);
            TransmitPipe.FlushBuffer();
            ReceivePipes.FlushBuffer();
        }

        public bool Write(byte[] data)
        {
            bool result = false;

            // Begin the write
            StartWrite(data);

            // ------------
            // At this point we could return from a non-blocking write, and then call
            // the rest after an interrupt

            // Instead, we are going to block here until we get TX_DS (transmission completed and ack'd)
            // or MAX_RT (maximum retries, transmission failed).  Also, we'll timeout in case the radio
            // is flaky and we get neither.

            // IN the end, the send should be blocking.  It comes back in 60ms worst case, or much faster
            // if I tighten up the retry logic.  (Default settings will be 1500us.
            // Monitor the send

            byte[] observeTx = new byte[1];
            byte status;
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            do
            {
                status = Configuration.Registers.ObserveTransmitRegister;
                //string observeTxDisplay = FormatObserveTx(observeTx[1]);
            } while ((status & (Utilities.BitValue(Properties.TX_DS) | Utilities.BitValue(Properties.MAX_RT))) != 1 && (stopwatch.ElapsedMilliseconds < 500));

            // The part above is what you could recreate with your own interrupt handler,
            // and then call this when you got an interrupt
            // ------------

            // Call this when you get an interrupt
            // The status tells us three things
            // * The send was successful (TX_DS)
            // * The send failed, too many retries (MAX_RT)
            // * There is an ack packet waiting (RX_DR)
            bool txOk, txFail;
            WhatHappened(out txOk, out txFail, out _isAckPayloadAvailable);
            result = txOk;

            // Handle the ack packet
            if (_isAckPayloadAvailable)
            {
                byte ackPayloadLength = Configuration.DynamicPayloadSize;
            }

            Status = DeviceStatus.PowerDown;
            TransmitPipe.FlushBuffer();

            return result;
        }

        public void StartWrite(byte[] data)
        {
            ConfigurationRegister configRegister = Configuration.Registers.ConfigurationRegister;
            configRegister.PWR_UP = true;
            configRegister.PRIM_RX = false;
            configRegister.Save();

            Utilities.DelayMicroseconds(150);

            WritePayload(data);

            ChipEnable(true);
            ChipEnable(false);
        }


        public bool Available()
        {
            return Available(null);
        }

        public bool Available(byte[] pipes)
        {
            StatusRegister statusRegister = Configuration.Registers.StatusRegister;

            bool result = statusRegister.RX_DR;
            if (result)
            {
                // If the caller wants the pipe number, include that
                if (pipes != null)
                    pipes[0] = (byte)((statusRegister >> Properties.RX_P_NO) & 0x7);

                statusRegister.RX_DR = true;

                // Handle ack payload receipt
                if (!statusRegister.TX_DS)
                    statusRegister.TX_DS = true;
                statusRegister.Save();
            }

            return result;
        }

        public bool Read(ref byte[] readBuffer, int length)
        {
            // Fetch the payload
            ReadPayload(ref readBuffer, length);

            // was this the last of the data available?
            return ReceivePipes.FifoStatus == FifoStatus.Empty;
        }

        public void WhatHappened(out bool txOk, out bool txFail, out bool rxReady)
        {
            StatusRegister statusRegister = Configuration.Registers.StatusRegister;
            statusRegister.RX_DR = true;
            statusRegister.TX_DS = true;
            statusRegister.MAX_RT = true;
            statusRegister.Save();

            txOk = statusRegister.TX_DS;
            txFail = statusRegister.MAX_RT;
            rxReady = statusRegister.RX_DR;
        }

        public void OpenWritingPipe(long value)
        {
            byte[] address = BitConverter.GetBytes(value);
            Array.Resize(ref address, 5);
            ReceivePipes[0].Address = address;
            ReceivePipes[0].PayloadWidth = Configuration.PayloadWidth;
            TransmitPipe.Address = address;
        }

        public void OpenReadingPipe(byte pipeId, long address)
        {
            byte[] buffer = BitConverter.GetBytes(address);
            if (pipeId == 0)
                _receiveAddressPipe0 = address;
            if (pipeId <= 1)
            {
                Array.Resize(ref buffer, 5);
            }
            else if (pipeId > 1 && pipeId < 6)
            {
                Array.Resize(ref buffer, 1);
            }
            ReceivePipe receivePipe = ReceivePipes[pipeId];
            receivePipe.Address = buffer;
            receivePipe.PayloadWidth = Configuration.PayloadWidth;
            receivePipe.Enabled = true;

        }

        public void EnableAckPayload()
        {
            FeatureRegister featureRegister = Configuration.Registers.FeatureRegister;
            featureRegister.EN_ACK_PAY = true;
            featureRegister.EN_DPL = true;
            featureRegister.Save();

            // If it didn't work, the features are not enabled
            if (featureRegister != 1)
            {
                // So enable them and try again
                Configuration.ToggleFeatures();

                featureRegister.EN_ACK_PAY = true;
                featureRegister.EN_DPL = true;
                featureRegister.Save();
            }

            DynamicPayloadLengthRegister dypRegister = Configuration.Registers.DynamicPayloadLengthRegister;
            dypRegister.DPL_P0 = true;
            dypRegister.DPL_P1 = true;
            dypRegister.Save();
        }

        public void WriteAckPayload(byte pipe, byte[] data, int length)
        {
            int payloadSize = Math.Min(length, Constants.MaxPayloadWidth);
            if (data.Length > payloadSize)
                Array.Resize(ref data, payloadSize);
            CommandProcessor.ExecuteCommand(DeviceCommands.W_ACK_PAYLOAD, (byte)(pipe & 0x7), data);
        }

        public bool ChannelReceivedPowerDector
        {
            get
            {
                Configuration.Registers.ReceivedPowerDetectorRegister.Load();
                return Configuration.Registers.ReceivedPowerDetectorRegister.RPD;
            }
        }

        private DeviceStatus _status;
        public DeviceStatus Status
        {
            get
            {
                return _status;
            }
            set
            {
                if (value == _status)
                    return;
                DeviceStatus previous = _status;
                _status = value;
                //clean up previous status
                //TODO: implement cleanup code
                switch (previous)
                {
                    case DeviceStatus.Undefined:
                        break;
                    case DeviceStatus.PowerDown:
                        break;
                    case DeviceStatus.StandBy:
                        break;
                    case DeviceStatus.TransmitMode:
                        break;
                    case DeviceStatus.ReceiveMode:
                        //Constants.CONFIG.PRIM_RX = false;  //disable RX role
                        //Constants.CONFIG.Save();
                        break;
                    default:
                        break;
                }
                //set device to new status
                switch (_status)
                {
                    case DeviceStatus.Undefined:
                        throw new InvalidOperationException("WTF???");
                    case DeviceStatus.PowerDown:
                        if (previous == DeviceStatus.StandBy)
                        {
                            Configuration.Registers.ConfigurationRegister.PWR_UP = false;
                            Configuration.Registers.ConfigurationRegister.Save();
                            break;
                        }
                        throw new InvalidOperationException(
                            "Error status change, PowerDown should from StandBy mode only");
                    case DeviceStatus.StandBy:
                        if (previous == DeviceStatus.ReceiveMode || previous == DeviceStatus.TransmitMode)
                        {
                            ChipEnable(false);
                            break;
                        }
                        if (previous == DeviceStatus.PowerDown)
                        {
                            Configuration.Registers.ConfigurationRegister.PWR_UP = true;
                            Configuration.Registers.ConfigurationRegister.Save();
                            Utilities.DelayMicroseconds(2000);
                            break;
                        }
                        throw new InvalidOperationException(
                            "Error status change, StandBy should from PowerDown,TX or RX mode only");
                    case DeviceStatus.TransmitMode:
                        if (previous == DeviceStatus.StandBy)
                        {
                            _checkStatus = false;
                            Configuration.Registers.ConfigurationRegister.PRIM_RX = false;
                            Configuration.Registers.ConfigurationRegister.Save();
                            _checkStatus = true;

                            ChipEnable(true);
                            Utilities.DelayMicroseconds(1000); //wait device to enter TX mode
                            break;
                        }
                        throw new InvalidOperationException("Error status change, RXMode should from Standyby mode only");
                    case DeviceStatus.ReceiveMode:
                        if (previous == DeviceStatus.StandBy)
                        {
                            _checkStatus = false;
                            Configuration.Registers.ConfigurationRegister.PRIM_RX = true;
                            Configuration.Registers.ConfigurationRegister.Save();
                            _checkStatus = false;
                            ChipEnable(true);
                            Utilities.DelayMicroseconds(1000); //wait device to enter RX mode
                            break;

                        }
                        throw new InvalidOperationException("Error status change, RXMode should from Standyby mode only");
                    default:
                        break;
                }
            }
        }
    }
}
