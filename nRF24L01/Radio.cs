using System;
using System.Diagnostics;
using Windows.Devices.Gpio;
using Windows.Devices.Radios.nRF24L01.Enums;
using Windows.Devices.Spi;

namespace Windows.Devices.Radios.nRF24L01
{
    /// <summary>
    /// Driver for nRF24L01(+) 2.4GHz Wireless Transceiver
    /// </summary>
    public class Radio
    {
        private long _pipe0ReadingAddress;
        private readonly object _spiLock;
        private readonly GpioPin _cePin;
        private readonly SpiDevice _spiDevice;
        private bool _checkStatus;

        public RadioConfig Configuration { get; private set; }
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

        public string Name => Constants.RadioModelStrings[(int)Configuration.RadioModel];

        public bool IsDataAvailable => (Configuration.GetStatus() & (Utilities.BitValue(Constants.MASK_RX_DR))) > 0;

        public Radio(GpioPin cePin, SpiDevice spiDevice)
        {
            _checkStatus = false;
            _isAckPayloadAvailable = false;
            _pipe0ReadingAddress = 0;
            _spiLock = new object();

            _spiDevice = spiDevice;
            _cePin = cePin;
            _cePin.SetDriveMode(GpioPinDriveMode.Output);

            Configuration = new RadioConfig(this);
            TransmitPipe = new TransmitPipe(this);
            ReceivePipes = new ReceivePipeCollection(this);
        }

        public byte Transfer(byte request)
        {
            if (_checkStatus && (request == Constants.W_REGISTER && !(_status == DeviceStatus.StandBy || _status == DeviceStatus.PowerDown)))
                throw new InvalidOperationException("Writing register should only be in Standby or PowerDown mode");
            byte[] response = new byte[1];
            lock (_spiLock)
            {
                _spiDevice.TransferFullDuplex(new[] { request }, response);
            }
            return response[0];
        }

        public byte[] Transfer(byte[] request)
        {
            if (_checkStatus && (request[0] == Constants.W_REGISTER && !(_status == DeviceStatus.StandBy || _status == DeviceStatus.PowerDown)))
                throw new InvalidOperationException("Writing register should only be in Standby or PowerDown mode");
            int length = request.Length;

            byte[] response = new byte[length],
                buffer = new byte[length];
            buffer[0] = request[0];
            for (int i = 1; i < request.Length; i++)
                buffer[length - i] = request[i];
            //Array.Reverse();
            lock (_spiLock)
            {
                _spiDevice.TransferFullDuplex(buffer, response);
            }

            for (int i = 0; i < request.Length; i++)
                buffer[(length - 1) - i] = response[i];

            return buffer;
        }

        public void ChipEnable(bool enabled)
        {
            _cePin.Write(enabled ? GpioPinValue.High : GpioPinValue.Low);
            Utilities.DelayMicroseconds(50);
        }

        protected byte WritePayload(byte[] payload)
        {
            int payloadSize = Math.Min(payload.Length, Configuration.PayloadSize);
            int blankLength = Configuration.DynamicPayloadLengthEnabled ? 0 : Configuration.PayloadSize - payloadSize;
            byte[] status = new byte[1];

            ChipEnable(false);
            status = Transfer(new[] { Constants.W_TX_PAYLOAD });

            Transfer(payload);
            for (int index = 0; index < blankLength; index++)
                Transfer(new byte[] { 0 });
            ChipEnable(true);

            return status[0];
        }

        protected byte ReadPayload(byte[] payload, int length)
        {
            int payloadSize = Math.Min(length, Configuration.PayloadSize);
            int blankLength = Configuration.DynamicPayloadLengthEnabled ? 0 : Configuration.PayloadSize - payloadSize;

            ChipEnable(false);

            byte[] status = Transfer(new[] { Constants.R_RX_PAYLOAD });

            byte[] request = new byte[payloadSize];
            for (int index = 0; index < payloadSize; index++)
                request[index] = Constants.NOP;
            payload = Transfer(request);

            for (int index = 0; index <= blankLength; index++)
                Transfer(new byte[] { Constants.NOP });

            ChipEnable(true);

            return status[0];
        }

        public void Begin()
        {
            Registers.RegisterCollection registers = Configuration.Registers;

            ChipEnable(false);

            // Must allow the radio time to settle else configuration bits will not necessarily stick.
            // This is actually only required following power up but some settling time also appears to
            // be required after resets too. For full coverage, we'll always assume the worst.
            // Enabling 16b CRC is by far the most obvious case if the wrong timing is used - or skipped.
            // Technically we require 4.5ms + 14us as a worst case. We'll just call it 5ms for good measure.
            // WARNING: Delay is based on P-variant whereby non-P *may* require different timing.
            Utilities.DelayMicroseconds(5000);

            // Set 1500uS (minimum for 32B payload in ESB@250KBPS) timeouts, to make testing a little easier
            // WARNING: If this is ever lowered, either 250KBS mode with AA is broken or maximum packet
            // sizes must never be used. See documentation for a more complete explanation.
            Configuration.AutoRetransmitDelay = AutoRetransmitDelay.Delay1500uS;
            Configuration.AutoRetransmitCount = 15;

            // Disable auto acknowledgement 
            Registers.AutoAckRegister autoAckRegister = registers.AutoAckRegister;
            autoAckRegister.EN_AA = false;
            autoAckRegister.Save();

            // Attempt to set DataRate to 250Kbps to determine if this is a plus model
            Configuration.DataRate = DataRates.DataRate250Kbps;
            Configuration.IsPlusModel = Configuration.DataRate == DataRates.DataRate250Kbps;

            // Restore our default PA level
            Configuration.PowerLevel = PowerLevels.PowerLevelMax;

            // Initialize CRC and request 2-byte (16bit) CRC
            Configuration.CrcEncodingScheme = CrcEncodingSchemes.DualBytes;
            Configuration.CrcEnabled = true;

            // Disable dynamic payloads, to match dynamic_payloads_enabled setting
            Configuration.DynamicPayloadLengthEnabled = false;

            // Set up default configuration.  Callers can always change it later.
            // This channel should be universally safe and not bleed over into adjacent spectrum.
            Configuration.Channel = 76;

            // Then set the data rate to the slowest (and most reliable) speed supported by all hardware.
            Configuration.DataRate = DataRates.DataRate1Mbps;

            // Reset current status
            // Notice reset and flush is the last thing we do
            Registers.StatusRegister statusRegister = registers.StatusRegister;
            statusRegister.RX_DR = true;
            statusRegister.TX_DS = true;
            statusRegister.MAX_RT = true;
            statusRegister.Save();

            TransmitPipe.FlushBuffer();
            ReceivePipes.FlushBuffer();
        }

        public void StartListening()
        {
            Registers.ConfigRegister configRegister = Configuration.Registers.ConfigRegister;
            configRegister.PWR_UP = true;
            configRegister.PRIM_RX = true;
            configRegister.Save();

            Registers.StatusRegister statusRegister = Configuration.Registers.StatusRegister;
            statusRegister.RX_DR = true;
            statusRegister.TX_DS = true;
            statusRegister.MAX_RT = true;
            statusRegister.Save();

            // Restore the pipe0 address, if exists
            if (_pipe0ReadingAddress > 0)
            {
                Registers.ReceivePipe0AddressRegister receivePipe0AddressRegister =
                    Configuration.Registers.ReceivePipe0AddressRegister;
                receivePipe0AddressRegister.RX_ADDR_P0 = BitConverter.GetBytes(_pipe0ReadingAddress);
                receivePipe0AddressRegister.Save();
            }

            TransmitPipe.FlushBuffer();
            ReceivePipes.FlushBuffer();

            ChipEnable(true);

            Utilities.DelayMicroseconds(130);
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
            // if I tighted up the retry logic.  (Default settings will be 1500us.
            // Monitor the send

            byte[] observeTx = new byte[1];
            byte status;
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            do
            {
                status = Configuration.ReadRegister(Constants.OBSERVE_TX, ref observeTx, 1);
                //string observeTxDisplay = FormatObserveTx(observeTx[1]);
            } while ((status & (Utilities.BitValue(Constants.TX_DS) | Utilities.BitValue(Constants.MAX_RT))) != 1 && (stopwatch.ElapsedMilliseconds < 500));

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
            Registers.ConfigRegister configRegister = Configuration.Registers.ConfigRegister;
            configRegister.PWR_UP = true;
            configRegister.PRIM_RX = false;
            configRegister.Save();

            Utilities.DelayMicroseconds(150);

            WritePayload(data);

            ChipEnable(true);
            Utilities.DelayMicroseconds(15);
            ChipEnable(false);
        }


        public bool Available()
        {
            return Available(null);
        }

        public bool Available(byte[] pipes)
        {
            Registers.StatusRegister statusRegister = Configuration.Registers.StatusRegister;

            bool result = statusRegister.RX_DR;
            if (result)
            {
                // If the caller wants the pipe number, include that
                if (pipes != null)
                    pipes[0] = (byte)((statusRegister.FirstByte >> Constants.RX_P_NO) & 0x7);

                statusRegister.RX_DR = true;

                // Handle ack payload receipt
                if (!statusRegister.TX_DS)
                    statusRegister.TX_DS = true;
                statusRegister.Save();
            }

            return result;
        }

        public bool Read(byte[] readBuffer, int length)
        {
            // Fetch the payload
            ReadPayload(readBuffer, length);

            // was this the last of the data available?
            return ReceivePipes.FifoStatus == FifoStatus.Empty;
        }

        public void WhatHappened(out bool txOk, out bool txFail, out bool rxReady)
        {
            Registers.StatusRegister statusRegister = Configuration.Registers.StatusRegister;
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
            ReceivePipes[0].PayloadWidth = Configuration.PayloadSize;
            TransmitPipe.Address = address;
        }

        public void OpenReadingPipe(byte pipeId, long address)
        {
            byte[] buffer = BitConverter.GetBytes(address);
            if (pipeId == 0)
                _pipe0ReadingAddress = address;
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
            receivePipe.PayloadWidth = Configuration.PayloadSize;
            receivePipe.Enabled = true;

        }


        public void EnableAckPayload()
        {
            // Enable ack payload and dynamic payload features
            Configuration.WriteRegister(Constants.FEATURE, (byte)(Configuration.ReadRegister(Constants.FEATURE) | Utilities.BitValue(Constants.EN_ACK_PAY) | Utilities.BitValue(Constants.EN_DPL)));

            // If it didn't work, the features are not enabled
            if (Configuration.ReadRegister(Constants.FEATURE) != 1)
            {
                // So enable them and try again
                Configuration.ToggleFeatures();
                Configuration.WriteRegister(Constants.FEATURE, (byte)(Configuration.ReadRegister(Constants.FEATURE) | Utilities.BitValue(Constants.EN_ACK_PAY) | Utilities.BitValue(Constants.EN_DPL)));
            }

            //
            // Enable dynamic payload on pipes 0 & 1
            //
            Configuration.WriteRegister(Constants.DYNPD, (byte)(Configuration.ReadRegister(Constants.DYNPD) | Utilities.BitValue(Constants.DPL_P1) | Utilities.BitValue(Constants.DPL_P0)));

        }

        public void WriteAckPayload(byte pipe, byte[] data, int length)
        {
            _spiDevice.Write(new[] { (byte)(Constants.W_ACK_PAYLOAD | (pipe & 0x7)) });
            int payloadSize = Math.Min(length, Constants.MaxPayloadSize);
            if (data.Length > payloadSize)
                Array.Resize(ref data, payloadSize);
            Transfer(data);
        }

        public bool TestCarrier()
        {
            return (Configuration.ReadRegister(Constants.CD) & 1) == 1;
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
                        //Registers.CONFIG.PRIM_RX = false;  //disable RX role
                        //Registers.CONFIG.Save();
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
                            Configuration.Registers.ConfigRegister.PWR_UP = false;
                            Configuration.Registers.ConfigRegister.Save();
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
                            Configuration.Registers.ConfigRegister.PWR_UP = true;
                            Configuration.Registers.ConfigRegister.Save();
                            Utilities.DelayMicroseconds(2000);
                            break;
                        }
                        throw new InvalidOperationException(
                            "Error status change, StandBy should from PowerDown,TX or RX mode only");
                    case DeviceStatus.TransmitMode:
                        if (previous == DeviceStatus.StandBy)
                        {
                            _checkStatus = false;
                            Configuration.Registers.ConfigRegister.PRIM_RX = false;
                            Configuration.Registers.ConfigRegister.Save();
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
                            Configuration.Registers.ConfigRegister.PRIM_RX = true;
                            Configuration.Registers.ConfigRegister.Save();
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
