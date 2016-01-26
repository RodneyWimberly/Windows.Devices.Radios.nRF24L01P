using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Gpio;
using Windows.Devices.Spi;

namespace nRF24L01
{
    /// <summary>
    /// Driver for nRF24L01(+) 2.4GHz Wireless Transceiver
    /// </summary>
    public class Radio : IRadio
    {
        private bool _isWideBand;

        private bool _dynamicPayloadEnabled;
        private long _pipe0ReadingAddress;

        private readonly GpioPin _cePin;
        private readonly SpiDevice _spiDevice;

        private byte _payloadSize;
        public byte PayloadSize
        {
            get { return _payloadSize; }
            set { _payloadSize = Math.Min(value, Constants.MaxPayloadSize); }
        }

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

        private bool _isPlusModel;
        public RadioModels RadioModel => _isPlusModel ? RadioModels.nRF24L01P : RadioModels.nRF24L01;
        public string RadioModelName => Constants.RadioModelStrings[(int)RadioModel];

        public bool IsDataAvailable => (GetStatus() & (_BV(Constants.MASK_RX_DR))) > 0;

        public Radio(GpioPin cePin, SpiDevice spiDevice)
        {
            _isPlusModel = false;
            _isWideBand = true;
            _payloadSize = Constants.MaxPayloadSize;
            _isAckPayloadAvailable = false;
            _dynamicPayloadEnabled = false;
            _pipe0ReadingAddress = 0;

            _spiDevice = spiDevice;
            _cePin = cePin;
            _cePin.SetDriveMode(GpioPinDriveMode.Output);
        }

        protected byte ReadRegister(byte register)
        {
            return ReadRegisters(register)[0];
        }

        protected byte[] ReadRegisters(byte register, int length = 0)
        {
            byte[] readBuffer = new byte[length + 1],
                writeBuffer = new byte[length + 1];

            writeBuffer[0] = (byte)(Constants.W_REGISTER | (Constants.REGISTER_MASK & register));
            for (int index = 1; index <= length; index++)
                writeBuffer[index] = Constants.NOP;

            _spiDevice.TransferFullDuplex(writeBuffer, readBuffer);

            return readBuffer;
        }

        protected void WriteRegister(byte register, byte value)
        {
            _spiDevice.Write(new[] { (byte)(Constants.W_REGISTER | (Constants.REGISTER_MASK & register)), value });
        }

        protected void WriteRegisters(byte register, byte[] values, int length)
        {
            Array.Resize(ref values, length);
            _spiDevice.Write(new[] { (byte)(Constants.W_REGISTER | (Constants.REGISTER_MASK & register)) });
            _spiDevice.Write(values);
        }

        protected void ChipEnable(bool enabled)
        {
            _cePin.Write(enabled ? GpioPinValue.High : GpioPinValue.Low);
            Task.Delay(50).Wait();
        }

        protected byte WritePayload(byte[] payload)
        {
            int payloadSize = Math.Min(payload.Length, _payloadSize);
            int blankLength = _dynamicPayloadEnabled ? 0 : _payloadSize - payloadSize;
            byte[] status = new byte[1];

            ChipEnable(false);
            _spiDevice.TransferFullDuplex(new[] { Constants.W_TX_PAYLOAD }, status);
            _spiDevice.Write(payload);
            for (int index = 0; index <= blankLength; index++)
                _spiDevice.Write(new byte[] { 0 });
            ChipEnable(true);

            return status[0];
        }

        protected byte ReadPayload(byte[] payload, int length)
        {
            int payloadSize = Math.Min(length, _payloadSize);
            int blankLength = _dynamicPayloadEnabled ? 0 : _payloadSize - payloadSize;

            ChipEnable(false);

            byte[] status = new byte[1];
            _spiDevice.TransferFullDuplex(new[] { Constants.R_RX_PAYLOAD }, status);

            byte[] request = new byte[payloadSize];
            for (int index = 0; index <= payloadSize; index++)
                request[index] = Constants.NOP;
            payload = new byte[payloadSize];
            _spiDevice.TransferFullDuplex(request, payload);

            //byte[] response = new byte[1];
            //payload = new byte[payloadSize];
            //for (int index = 0; index <= payloadSize; index++)
            //{
            //    _spiDevice.TransferFullDuplex(new[] {Constants.NOP}, response);
            //    payload[index] = response[0];
            //}

            for (int index = 0; index <= blankLength; index++)
                _spiDevice.Write(new byte[] { Constants.NOP });
            ChipEnable(true);

            return status[0];
        }

        protected void FlushTransmitBuffer()
        {
            _spiDevice.Write(new[] { Constants.FLUSH_TX });
        }

        protected void FlushReceiveBuffer()
        {
            _spiDevice.Write(new[] { Constants.FLUSH_RX });
        }

        protected byte GetStatus()
        {
            byte[] readBuffer = new byte[1];
            _spiDevice.TransferFullDuplex(new[] { Constants.NOP }, readBuffer);

            return readBuffer[0];
        }

        protected string GetAddressRegister(string name, byte register, int quantity)
        {
            string registerValue = "\t" + name + (name.Length < 8 ? "\t" : "") + " = ";
            while (quantity-- >= 0)
            {
                byte[] values = ReadRegisters(register++, 5);
                registerValue += " 0x" + BitConverter.ToString(values).Replace("-", string.Empty);
            }

            return registerValue;
        }

        protected string GetByteRegister(string name, byte register, int quantity)
        {
            string registerValue = "\t" + name + (name.Length < 8 ? "\t" : "") + " = ";
            while (quantity-- >= 0)
            {
                byte[] values = ReadRegisters(register++, 1);
                registerValue += " 0x" + BitConverter.ToString(values).Replace("-", string.Empty);
            }

            return registerValue;
        }

        public void Begin()
        {
            ChipEnable(false);

            // Must allow the radio time to settle else configuration bits will not necessarily stick.
            // This is actually only required following power up but some settling time also appears to
            // be required after resets too. For full coverage, we'll always assume the worst.
            // Enabling 16b CRC is by far the most obvious case if the wrong timing is used - or skipped.
            // Technically we require 4.5ms + 14us as a worst case. We'll just call it 5ms for good measure.
            // WARNING: Delay is based on P-variant whereby non-P *may* require different timing.
            Task.Delay(5).Wait();

            // Set 1500uS (minimum for 32B payload in ESB@250KBPS) timeouts, to make testing a little easier
            // WARNING: If this is ever lowered, either 250KBS mode with AA is broken or maximum packet
            // sizes must never be used. See documentation for a more complete explanation.
            SetRetries(0x04, 0x0F);

            // Set payload sizes
            WriteRegister(Constants.RX_PW_P0, PayloadSize);
            WriteRegister(Constants.RX_PW_P1, PayloadSize);

            // Disable auto acknowledgement 
            SetAutoAck(false);

            // Restore our default PA level
            SetPowerLevel(PowerLevels.PowerLevelMax);

            // Determine if this is a p or non-p RF24 module and then reset our data rate back to default 
            // value. This works because a non-P variant won't allow the data rate to be set to 250Kbps.
            _isPlusModel = SetDataRate(DataRates.DataRate250Kbps);

            // Then set the data rate to the slowest (and most reliable) speed supported by all hardware.
            SetDataRate(DataRates.DataRate1Mbps);

            // Initialize CRC and request 2-byte (16bit) CRC
            SetCrcLength(CrcLengths.Crc16Bit);

            // Disable dynamic payloads, to match dynamic_payloads_enabled setting
            WriteRegister(Constants.DYNPD, 0);

            // Reset current status
            // Notice reset and flush is the last thing we do
            WriteRegister(Constants.STATUS, (byte)(_BV(Constants.RX_DR) | _BV(Constants.TX_DS) | _BV(Constants.MAX_RT)));

            // Set up default configuration.  Callers can always change it later.
            // This channel should be universally safe and not bleed over into adjacent spectrum.
            SetChannel(76);

            FlushTransmitBuffer();
            FlushReceiveBuffer();

            //OperationalMode = OperationalModes.Receive;
        }

        public void StartListening()
        {
            WriteRegister(Constants.CONFIG, (byte)(ReadRegister(Constants.CONFIG) | _BV(Constants.PWR_UP) | _BV(Constants.PRIM_RX)));
            WriteRegister(Constants.STATUS, (byte)(_BV(Constants.RX_DR) | _BV(Constants.TX_DS) | _BV(Constants.MAX_RT)));

            // Restore the pipe0 address, if exists
            if (_pipe0ReadingAddress > 0)
                WriteRegisters(Constants.RX_ADDR_P0, BitConverter.GetBytes(_pipe0ReadingAddress), 5);

            // Flush buffers
            FlushReceiveBuffer();
            FlushTransmitBuffer();

            // Go!
            ChipEnable(true);

            // wait for the radio to come up (130us actually only needed)
            Task.Delay(1).Wait();
        }

        public void StopListening()
        {
            ChipEnable(false);
            FlushTransmitBuffer();
            FlushReceiveBuffer();
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

            byte[] observeTx;
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            do
            {
                observeTx = ReadRegisters(Constants.OBSERVE_TX, 1);
                //string observeTxDisplay = FormatObserveTx(observeTx[1]);
            } while ((observeTx[0] & (_BV(Constants.TX_DS) | _BV(Constants.MAX_RT))) != 1 && (stopwatch.ElapsedMilliseconds < 500));

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
                byte ackPayloadLength = GetDynamicPayloadSize();
            }

            PowerDown();
            FlushTransmitBuffer();

            return result;
        }

        public void StartWrite(byte[] data)
        {
            WriteRegister(Constants.CONFIG, (byte)(ReadRegister(Constants.CONFIG) | _BV(Constants.PWR_UP) & ~_BV(Constants.PRIM_RX)));
            DelayMicroseconds(150);

            WritePayload(data);

            ChipEnable(true);
            DelayMicroseconds(15);
            ChipEnable(false);
        }

        private string FormatObserveTx(byte value)
        {
            return string.Format("OBSERVE_TX={0:X2}: POLS_CNT={1} ARC_CNT={2}",
                value,
                (value >> Constants.PLOS_CNT) & 0xF,
                (value >> Constants.ARC_CNT) & 0xF);
        }

        public byte GetDynamicPayloadSize()
        {
            byte[] result = new byte[1];

            _spiDevice.TransferFullDuplex(new[] { Constants.R_RX_PL_WID, Constants.NOP }, result);
            return result[0];
        }

        public bool Available()
        {
            return Available(null);
        }

        public bool Available(byte[] pipes)
        {
            byte status = GetStatus();
            bool result = (status & _BV(Constants.RX_DR)) == 1;
            if (result)
            {
                // If the caller wants the pipe number, include that
                if (pipes != null)
                    pipes[0] = (byte)((status >> Constants.RX_P_NO) & 0x7);

                // Clear the status bit

                // ??? Should this REALLY be cleared now?  Or wait until we
                // actually READ the payload?
                WriteRegister(Constants.STATUS, _BV(Constants.RX_DR));

                // Handle ack payload receipt
                if ((status & _BV(Constants.TX_DS)) == 1)
                    WriteRegister(Constants.STATUS, _BV(Constants.TX_DS));
            }

            return result;
        }

        public bool Read(byte[] readBuffer, int length)
        {
            // Fetch the payload
            ReadPayload(readBuffer, length);

            // was this the last of the data available?
            return (ReadRegister(Constants.FIFO_STATUS) & _BV(Constants.RX_EMPTY)) == 1;
        }

        public void WhatHappened(out bool txOk, out bool txFail, out bool rxReady)
        {
            byte status = ReadRegister(Constants.STATUS);
            WriteRegister(Constants.STATUS, (byte)(_BV(Constants.RX_DR) | _BV(Constants.TX_DS) | _BV(Constants.MAX_RT)));

            txOk = (status & _BV(Constants.TX_DS)) == 1;
            txFail = (status & _BV(Constants.MAX_RT)) == 1;
            rxReady = (status & _BV(Constants.RX_DR)) == 1;
        }

        public void OpenWritingPipe(long value)
        {
            WriteRegisters(Constants.RX_ADDR_P0, BitConverter.GetBytes(value), 5);
            WriteRegisters(Constants.TX_ADDR, BitConverter.GetBytes(value), 5);
            WriteRegister(Constants.RX_PW_P0, PayloadSize);
        }

        public void OpenReadingPipe(byte child, long address)
        {
            if (child == 0)
                _pipe0ReadingAddress = address;
            if (child <= 6)
            {
                // For pipes 2-5, only write the LSB
                if (child < 2)
                    WriteRegisters(Constants.ChildPipes[child], BitConverter.GetBytes(address), 5);
                else
                    WriteRegisters(Constants.ChildPipes[child], BitConverter.GetBytes(address), 1);

                WriteRegister(Constants.ChildPayloadSizes[child], PayloadSize);

                // Note it would be more efficient to set all of the bits for all open
                // pipes at once.  However, I thought it would make the calling code
                // more simple to do it this way.
                WriteRegister(Constants.EN_RXADDR, (byte)(ReadRegister(Constants.EN_RXADDR) | _BV(Constants.ChildPipeEnable[child])));
            }
        }

        public void EnableDynamicPayloads()
        {
            // Enable dynamic payload throughout the system
            WriteRegister(Constants.FEATURE, (byte)(ReadRegister(Constants.FEATURE) | _BV(Constants.EN_DPL)));

            // If it didn't work, the features are not enabled
            if (ReadRegister(Constants.FEATURE) != 1)
            {
                // So enable them and try again
                ToggleFeatures();
                WriteRegister(Constants.FEATURE, (byte)(ReadRegister(Constants.FEATURE) | _BV(Constants.EN_DPL)));
            }

            // Enable dynamic payload on all pipes
            // Not sure the use case of only having dynamic payload on certain pipes, so the library does not support it.
            WriteRegister(Constants.DYNPD, (byte)(ReadRegister(Constants.DYNPD) | _BV(Constants.DPL_P5) | _BV(Constants.DPL_P4)
                | _BV(Constants.DPL_P3) | _BV(Constants.DPL_P2) | _BV(Constants.DPL_P1) | _BV(Constants.DPL_P0)));
            _dynamicPayloadEnabled = true;
        }

        public void EnableAckPayload()
        {
            // Enable ack payload and dynamic payload features
            WriteRegister(Constants.FEATURE, (byte)(ReadRegister(Constants.FEATURE) | _BV(Constants.EN_ACK_PAY) | _BV(Constants.EN_DPL)));

            // If it didn't work, the features are not enabled
            if (ReadRegister(Constants.FEATURE) != 1)
            {
                // So enable them and try again
                ToggleFeatures();
                WriteRegister(Constants.FEATURE, (byte)(ReadRegister(Constants.FEATURE) | _BV(Constants.EN_ACK_PAY) | _BV(Constants.EN_DPL)));
            }

            //
            // Enable dynamic payload on pipes 0 & 1
            //
            WriteRegister(Constants.DYNPD, (byte)(ReadRegister(Constants.DYNPD) | _BV(Constants.DPL_P1) | _BV(Constants.DPL_P0)));

        }

        public void WriteAckPayload(byte pipe, byte[] data, int length)
        {
            _spiDevice.Write(new[] { (byte)(Constants.W_ACK_PAYLOAD | (pipe & 0x7)) });
            int payloadSize = Math.Min(length, Constants.MaxPayloadSize);
            if (data.Length > payloadSize)
                Array.Resize(ref data, payloadSize);
            _spiDevice.Write(data);
        }

        public void ToggleFeatures()
        {
            _spiDevice.Write(new byte[] { Constants.ACTIVATE, 0x73 });

        }

        public void PowerDown()
        {
            WriteRegister(Constants.CONFIG, (byte)(ReadRegister(Constants.CONFIG) & ~_BV(Constants.PWR_UP)));
        }

        public void PowerUp()
        {
            WriteRegister(Constants.CONFIG, (byte)(ReadRegister(Constants.CONFIG) | _BV(Constants.PWR_UP)));
        }

        public void SetChannel(byte channel)
        {
            // TODO: This method could take advantage of the 'wide_band' calculation
            // done in setChannel() to require certain channel spacing.
            WriteRegister(Constants.RF_CH, Math.Min(channel, Constants.MaxChannel));
        }

        public bool TestCarrier()
        {
            return (ReadRegister(Constants.CD) & 1) == 1;
        }

        public bool TestRpd()
        {
            return (ReadRegister(Constants.RPD) & 1) == 1;
        }

        public string GetDetails()
        {
            StringBuilder sb = new StringBuilder();

            byte status = GetStatus();
            sb.AppendFormat("STATUS\t\t = 0x{0} RX_DR={1} TX_DS={2} MAX_RT={3} RX_P_NO={4} TX_FULL={5}\r\n",
                status,
                status & _BV(Constants.RX_DR),
                status & _BV(Constants.TX_DS),
                status & _BV(Constants.MAX_RT),
                (status >> Constants.RX_P_NO) & 7,
                status & _BV(Constants.TX_FULL));

            sb.AppendLine(GetAddressRegister("RX_ADDR_P0-1", Constants.RX_ADDR_P0, 2));
            sb.AppendLine(GetByteRegister("RX_ADDR_P2-5", Constants.RX_ADDR_P2, 4));
            sb.AppendLine(GetAddressRegister("TX_ADDR", Constants.TX_ADDR, 1));

            sb.AppendLine(GetByteRegister("RX_PW_P0-6", Constants.RX_PW_P0, 6));
            sb.AppendLine(GetByteRegister("EN_AA", Constants.EN_AA, 1));
            sb.AppendLine(GetByteRegister("EN_RXADDR", Constants.EN_RXADDR, 1));
            sb.AppendLine(GetByteRegister("RF_CH", Constants.RF_CH, 1));
            sb.AppendLine(GetByteRegister("RF_SETUP", Constants.RF_SETUP, 1));
            sb.AppendLine(GetByteRegister("CONFIG", Constants.CONFIG, 1));
            sb.AppendLine(GetByteRegister("DYNPD/FEATURE", Constants.DYNPD, 2));

            sb.AppendLine("Data Rate\t = " + Constants.DataRateStrings[(int)GetDataRate()]);
            sb.AppendLine("Model\t\t = " + RadioModelName);
            sb.AppendLine("CRC Length\t = " + Constants.CrcLengthStrings[(int)GetCrcLength()]);
            sb.AppendLine("PA Power\t = " + Constants.PowerLevelStrings[(int)GetPowerLevel()]);

            Debug.WriteLine(sb);
            return sb.ToString();
        }

        public void SetAutoAck(bool enable)
        {
            WriteRegister(Constants.EN_AA, (byte)(enable ? 0x3F : 0x0));
        }

        public void SetAutoAck(byte pipe, bool enable)
        {
            if (pipe > 6) return;
            byte enAa = ReadRegister(Constants.EN_AA);
            if (enable)
                enAa |= _BV(pipe);
            else
                enAa &= (byte)~_BV(pipe);
            WriteRegister(Constants.EN_AA, enAa);
        }

        public DataRates GetDataRate()
        {
            DataRates dataRate = DataRates.DataRate250Kbps;
            byte setup = (byte)(ReadRegister(Constants.RF_SETUP) & (1 << Constants.RF_DR_LOW | 1 << Constants.RF_DR_HIGH));
            if (setup == _BV(Constants.RF_DR_LOW))
                dataRate = DataRates.DataRate250Kbps;
            else if (setup == _BV(Constants.RF_DR_HIGH))
                dataRate = DataRates.DataRate2Mbps;
            else
                dataRate = DataRates.DataRate1Mbps;
            return dataRate;
        }

        public bool SetDataRate(DataRates dataRate)
        {
            bool success = false;
            byte setup = ReadRegister(Constants.RF_SETUP);
            // HIGH and LOW '00' is 1Mbs - our default
            _isWideBand = false;
            setup &= (byte)(~(_BV(Constants.RF_DR_LOW) | _BV(Constants.RF_DR_HIGH)));
            if (dataRate == DataRates.DataRate250Kbps)
            {
                // Must set the RF_DR_LOW to 1; RF_DR_HIGH (used to be RF_DR) is already 0
                // Making it '10'.
                _isWideBand = false;
                setup |= _BV(Constants.RF_DR_LOW);
            }
            else
            {
                // Set 2Mbs, RF_DR (RF_DR_HIGH) is set 1
                // Making it '01'
                if (dataRate == DataRates.DataRate2Mbps)
                {
                    _isWideBand = true;
                    setup |= _BV(Constants.RF_DR_HIGH);
                }
                else
                {
                    // 1Mbs
                    _isWideBand = false;
                }
            }
            WriteRegister(Constants.RF_SETUP, setup);

            // Verify Results
            if (ReadRegister(Constants.RF_SETUP) == setup)
                success = true;
            else
                _isWideBand = false;

            return success;
        }

        public CrcLengths GetCrcLength()
        {
            CrcLengths crcLength = CrcLengths.CrcDisabled;
            byte config = (byte)(ReadRegister(Constants.CONFIG) & (_BV(Constants.CRCO) | _BV(Constants.EN_CRC)));
            if ((config & _BV(Constants.EN_CRC)) == 1)
                crcLength = (config & _BV(Constants.CRCO)) == 1 ? CrcLengths.Crc16Bit : CrcLengths.Crc8Bit;

            return crcLength;
        }

        public void SetCrcLength(CrcLengths crcLength)
        {
            byte config = (byte)(ReadRegister(Constants.CONFIG) & ~(_BV(Constants.CRCO) | _BV(Constants.CRCO)));
            if (crcLength == CrcLengths.CrcDisabled)
            {
                config = (byte)(ReadRegister(Constants.CONFIG) & ~_BV(Constants.EN_CRC));
            }
            else if (crcLength == CrcLengths.Crc8Bit)
            {
                config |= _BV(Constants.EN_CRC);
            }
            else
            {
                config |= _BV(Constants.EN_CRC);
                config |= _BV(Constants.CRCO);
            }

            WriteRegister(Constants.CONFIG, config);
        }

        public PowerLevels GetPowerLevel()
        {
            PowerLevels powerLevel = PowerLevels.PowerLevelError;
            byte setup = (byte)(ReadRegister(Constants.RF_SETUP) & (_BV(Constants.RF_PWR_LOW) | _BV(Constants.RF_PWR_HIGH)));
            if (setup == (_BV(Constants.RF_PWR_LOW) | _BV(Constants.RF_PWR_HIGH)))
                powerLevel = PowerLevels.PowerLevelMax;
            else if (setup == _BV(Constants.RF_PWR_HIGH))
                powerLevel = PowerLevels.PowerLevelHigh;
            else if (setup == _BV(Constants.RF_PWR_LOW))
                powerLevel = PowerLevels.PowerLevelLow;
            else
                powerLevel = PowerLevels.PowerLevelMin;
            return powerLevel;
        }

        public void SetPowerLevel(PowerLevels powerLevel)
        {
            byte setup = ReadRegister(Constants.RF_SETUP);
            setup &= (byte)(~(_BV(Constants.RF_PWR_LOW) | _BV(Constants.RF_PWR_HIGH)));
            if (powerLevel == PowerLevels.PowerLevelMax)
                setup |= (byte)(_BV(Constants.RF_PWR_LOW) | _BV(Constants.RF_PWR_HIGH));
            else if (powerLevel == PowerLevels.PowerLevelHigh)
                setup |= _BV(Constants.RF_PWR_HIGH);
            else if (powerLevel == PowerLevels.PowerLevelLow)
                setup |= _BV(Constants.RF_PWR_LOW);
            else if (powerLevel == PowerLevels.PowerLevelMin)
            {
                // nothing
            }
            else if (powerLevel == PowerLevels.PowerLevelError)
                setup |= (byte)(_BV(Constants.RF_PWR_LOW) | _BV(Constants.RF_PWR_HIGH));

            WriteRegister(Constants.RF_SETUP, setup);
        }

        public void SetRetries(byte delay, byte count)
        {
            WriteRegister(Constants.SETUP_RETR, (byte)((delay & 0xf) << Constants.ARD | (count & 0xf) << Constants.ARC));
        }

        private static byte _BV(byte mask)
        {
            return (byte)(1 << mask);
        }

        private static void DelayMicroseconds(int microSeconds)
        {
            // We will assume that 10 ticks = 1 microsecond
            Task.Delay(new TimeSpan(10 * microSeconds)).Wait();
        }
    }
}
