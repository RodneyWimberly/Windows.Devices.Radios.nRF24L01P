using System;
using System.Text;
using Windows.Devices.Radios.nRF24L01.Enums;
using Windows.Devices.Radios.nRF24L01.Registers;

namespace Windows.Devices.Radios.nRF24L01
{
    public class RadioConfig
    {
        private readonly Radio _radio;
        public RegisterCollection Registers { get; private set; }

        public RadioConfig(Radio radio)
        {
            _radio = radio;
            _payloadSize = Constants.MaxPayloadSize;
            Registers = new RegisterCollection(_radio);
            Registers.LoadAll();
        }

        public RadioKind RadioKind => RadioKind.Other;

        public DataRates DataRate
        {
            get
            {
                DataRates dataRate = DataRates.DataRate1Mbps;

                bool low = Registers.SetupRegister.RF_DR_LOW;
                bool high = Registers.SetupRegister.RF_DR_HIGH;
                if (low)
                    dataRate = DataRates.DataRate250Kbps;
                else if (high)
                    dataRate = DataRates.DataRate2Mbps;

                return dataRate;
            }
            set
            {
                switch (value)
                {
                    case DataRates.DataRate1Mbps:
                        Registers.SetupRegister.RF_DR_LOW = false;
                        Registers.SetupRegister.RF_DR_HIGH = false;
                        break;
                    case DataRates.DataRate2Mbps:
                        Registers.SetupRegister.RF_DR_LOW = false;
                        Registers.SetupRegister.RF_DR_HIGH = true;
                        break;
                    case DataRates.DataRate250Kbps:
                        Registers.SetupRegister.RF_DR_LOW = true;
                        Registers.SetupRegister.RF_DR_HIGH = false;
                        break;
                    default:
                        throw new InvalidOperationException("Invalid data rate value");
                }
                Registers.SetupRegister.Save();
            }
        }

        public PowerLevels PowerLevel
        {
            get
            {
                PowerLevels powerLevel = PowerLevels.PowerLevelError;
                bool low = Registers.SetupRegister.RF_DR_LOW;
                bool high = Registers.SetupRegister.RF_DR_HIGH;
                if (low && high)
                    powerLevel = PowerLevels.PowerLevelMax;
                else if (high)
                    powerLevel = PowerLevels.PowerLevelHigh;
                else if (low)
                    powerLevel = PowerLevels.PowerLevelLow;
                else
                    powerLevel = PowerLevels.PowerLevelMin;
                return powerLevel;
            }
            set
            {
                switch (value)
                {
                    case PowerLevels.PowerLevelMax:
                        Registers.SetupRegister.RF_DR_LOW = true;
                        Registers.SetupRegister.RF_DR_HIGH = true;
                        break;
                    case PowerLevels.PowerLevelHigh:
                        Registers.SetupRegister.RF_DR_LOW = false;
                        Registers.SetupRegister.RF_DR_HIGH = true;
                        break;
                    case PowerLevels.PowerLevelLow:
                        Registers.SetupRegister.RF_DR_LOW = true;
                        Registers.SetupRegister.RF_DR_HIGH = false;
                        break;
                    case PowerLevels.PowerLevelMin:
                        Registers.SetupRegister.RF_DR_LOW = false;
                        Registers.SetupRegister.RF_DR_HIGH = false;
                        break;
                }
                Registers.SetupRegister.Save();
            }
        }

        public uint Channel
        {
            get
            {
                return Registers.ChannelRegister.RF_CH;
            }
            set
            {
                if (Channel > Constants.MaxChannel)
                    throw new ArgumentOutOfRangeException(string.Format("Channel", "Channel should be 0 - {0}", Constants.MaxChannel));
                Registers.ChannelRegister.RF_CH = (byte)value;
                Registers.ChannelRegister.Save();
            }
        }

        public bool IsPlusModel;
        public RadioModels RadioModel => IsPlusModel ? RadioModels.nRF24L01P : RadioModels.nRF24L01;

        public bool CrcEnabled
        {
            get
            {
                return Registers.ConfigRegister.EN_CRC;
            }
            set
            {
                Registers.ConfigRegister.EN_CRC = value;
                Registers.ConfigRegister.Save();
            }
        }

        public CrcEncodingSchemes CrcEncodingScheme
        {
            get
            {
                return Registers.ConfigRegister.CRCO ? CrcEncodingSchemes.DualBytes : CrcEncodingSchemes.SingleByte;
            }
            set
            {
                Registers.ConfigRegister.CRCO = (value == CrcEncodingSchemes.DualBytes);
                Registers.ConfigRegister.Save();
            }
        }

        public AutoRetransmitDelay AutoRetransmitDelay
        {
            get
            {
                return (AutoRetransmitDelay)Registers.SetupRetryRegister.ARD;
            }
            set
            {
                Registers.SetupRetryRegister.ARD = (byte)value;
                Registers.SetupRetryRegister.Save();
            }
        }

        public byte AutoRetransmitCount
        {
            get
            {
                return Registers.SetupRetryRegister.ARC;
            }
            set
            {
                if (value > 15)
                    throw new ArgumentOutOfRangeException(nameof(AutoRetransmitCount), "AutoRetransmitCount should be 0 - 15");
                Registers.SetupRetryRegister.ARC = value;
                Registers.SetupRetryRegister.Save();
            }
        }

        public int AddressWidth
        {
            get
            {
                switch ((int)Registers.AddressWidthRegister.AW)
                {
                    case 1:
                        return 3;   //01 = 3 bytes
                    case 2:
                        return 4;   //10 = 4 bytes
                    case 3:
                        return 5;   //11 = 5 bytes
                    default:
                        throw new InvalidOperationException("Error reading registry value AddressWidthRegister.AW");
                }
            }
            set
            {
                switch (value)
                {
                    case 3:
                        Registers.AddressWidthRegister.AW = (byte)1;
                        break;
                    case 4:
                        Registers.AddressWidthRegister.AW = (byte)2;
                        break;
                    case 5:
                        Registers.AddressWidthRegister.AW = (byte)3;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("AddressWidth", "AddressWidth should be 3,4,5 only");
                }
                Registers.AddressWidthRegister.Save();
            }
        }

        public bool DynamicPayloadLengthEnabled
        {
            get
            {
                return Registers.FeatureRegister.EN_DPL;
            }
            set
            {
                Registers.FeatureRegister.EN_DPL = value;
                Registers.FeatureRegister.Save();
            }
        }

        public byte DynamicPayloadSize
        {
            get
            {
                byte[] result = _radio.Transfer(new[] { Constants.R_RX_PL_WID, Constants.NOP });
                return result[0];
            }
        }

        private byte _payloadSize;

        public byte PayloadSize
        {
            get { return _payloadSize; }
            set { _payloadSize = Math.Min(value, Constants.MaxPayloadSize); }
        }

        public void ToggleFeatures()
        {
            _radio.Transfer(new byte[] { Constants.ACTIVATE, 0x73 });
        }

        public byte ReadRegister(byte register)
        {
            byte[] request = new byte[2],
                response = new byte[1];

            request[0] = (byte)(Constants.R_REGISTER | (Constants.REGISTER_MASK & register));
            response = _radio.Transfer(request);
            return response[0];
        }

        public byte ReadRegister(byte register, ref byte[] buffer, int length = 0)
        {
            byte[] request = new byte[length + 1],
             response = new byte[length];
            request[0] = (byte)(Constants.R_REGISTER | (Constants.REGISTER_MASK & register));
            Array.Copy(buffer, 0, request, 1, length);
            response = _radio.Transfer(request);
            buffer = new byte[response.Length - 1];
            Array.Copy(response, 0, buffer, 0, buffer.Length);
            return response[0];
        }

        public byte WriteRegister(byte register, byte value)
        {
            byte[] status = _radio.Transfer(new[] { (byte)(Constants.W_REGISTER | (Constants.REGISTER_MASK & register)), value });
            _radio.Transfer(value);
            return status[0];
        }

        public byte WriteRegister(byte register, byte[] values, int length)
        {
            byte[] buffer = new byte[length + 1];
            buffer[0] = (byte)(Constants.W_REGISTER | (Constants.REGISTER_MASK & register));
            Array.Copy(values, 0, buffer, 1, length);
            byte[] status = _radio.Transfer(buffer);
            return status[0];
        }

        private string FormatObserveTx(byte value)
        {
            return string.Format("OBSERVE_TX={0:X2}: POLS_CNT={1} ARC_CNT={2}",
                value,
                (value >> Constants.PLOS_CNT) & 0xF,
                (value >> Constants.ARC_CNT) & 0xF);
        }

        public byte GetStatus()
        {
            return ReadRegister(Constants.STATUS);
        }

        private string GetAddressRegister(string name, byte register, int quantity)
        {
            string registerValue = name + " =";
            for (int index = 0; index < quantity; index++)
            {
                byte[] buffer = new byte[5];
                ReadRegister(register++, ref buffer, buffer.Length);
                registerValue += " 0x" + BitConverter.ToString(buffer).Replace("-", string.Empty);
            }

            return registerValue;
        }

        private string GetByteRegister(string name, byte register, int quantity)
        {
            string registerValue = name + " =";
            for (int index = 0; index < quantity; index++)
            {
                registerValue += " 0x" + ReadRegister(register++);
            }

            return registerValue;
        }

        public string GetDetails()
        {
            StringBuilder sb = new StringBuilder();

            byte status = GetStatus();
            sb.AppendFormat("STATUS = 0x{0} RX_DR={1} TX_DS={2} MAX_RT={3} RX_P_NO={4} TX_FULL={5}\r\n",
                status,
                (status & Constants.RX_DR) > 0,
                (status & Constants.TX_DS) > 0,
                (status & Constants.MAX_RT) > 0,
                (status << (4) >> (5)),
                (status & Constants.TX_FULL) > 0);

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

            sb.AppendLine("Data Rate = " + Constants.DataRateStrings[(int)DataRate]);
            sb.AppendLine("Model = " + _radio.Name);
            sb.AppendLine("CRC Enabled = " + CrcEnabled);
            sb.AppendLine("CRC Encoding Scheme = " + Constants.CrcEncodingSchemeStrings[(int)CrcEncodingScheme]);
            sb.AppendLine("PA Power = " + Constants.PowerLevelStrings[(int)PowerLevel]);

            sb.AppendFormat("\r\n{0}\r\n", Registers);
            return sb.ToString();
        }

        public override string ToString()
        {
            return Registers.ToString();
        }
    }
}
