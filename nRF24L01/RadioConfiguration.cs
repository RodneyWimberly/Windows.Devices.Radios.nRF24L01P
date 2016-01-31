using System;
using System.Text;
using Windows.Devices.Radios.nRF24L01.Enums;
using Windows.Devices.Radios.nRF24L01.Registers;

namespace Windows.Devices.Radios.nRF24L01
{
    public class RadioConfiguration
    {
        private readonly Radio _radio;
        public RegisterCollection Registers { get; private set; }

        public RadioConfiguration(Radio radio)
        {
            _radio = radio;
            _payloadSize = Constants.MaxPayloadWidth;
            Registers = new RegisterCollection(_radio);
            Registers.LoadAll();
        }

        public RadioKind RadioKind => RadioKind.Other;

        public DataRates DataRate
        {
            get
            {
                DataRates dataRate = DataRates.DataRate1Mbps;

                bool low = Registers.RfSetupRegister.RF_DR_LOW;
                bool high = Registers.RfSetupRegister.RF_DR_HIGH;
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
                        Registers.RfSetupRegister.RF_DR_LOW = false;
                        Registers.RfSetupRegister.RF_DR_HIGH = false;
                        break;
                    case DataRates.DataRate2Mbps:
                        Registers.RfSetupRegister.RF_DR_LOW = false;
                        Registers.RfSetupRegister.RF_DR_HIGH = true;
                        break;
                    case DataRates.DataRate250Kbps:
                        Registers.RfSetupRegister.RF_DR_LOW = true;
                        Registers.RfSetupRegister.RF_DR_HIGH = false;
                        break;
                    default:
                        throw new InvalidOperationException("Invalid data rate value");
                }
                Registers.RfSetupRegister.Save();
            }
        }

        public PowerLevels PowerLevel
        {
            get
            {
                PowerLevels powerLevel = PowerLevels.Error;
                bool low = Registers.RfSetupRegister.RF_DR_LOW;
                bool high = Registers.RfSetupRegister.RF_DR_HIGH;
                if (low && high)
                    powerLevel = PowerLevels.Max;
                else if (high)
                    powerLevel = PowerLevels.High;
                else if (low)
                    powerLevel = PowerLevels.Low;
                else
                    powerLevel = PowerLevels.Min;
                return powerLevel;
            }
            set
            {
                switch (value)
                {
                    case PowerLevels.Max:
                        Registers.RfSetupRegister.RF_DR_LOW = true;
                        Registers.RfSetupRegister.RF_DR_HIGH = true;
                        break;
                    case PowerLevels.High:
                        Registers.RfSetupRegister.RF_DR_LOW = false;
                        Registers.RfSetupRegister.RF_DR_HIGH = true;
                        break;
                    case PowerLevels.Low:
                        Registers.RfSetupRegister.RF_DR_LOW = true;
                        Registers.RfSetupRegister.RF_DR_HIGH = false;
                        break;
                    case PowerLevels.Min:
                        Registers.RfSetupRegister.RF_DR_LOW = false;
                        Registers.RfSetupRegister.RF_DR_HIGH = false;
                        break;
                }
                Registers.RfSetupRegister.Save();
            }
        }

        public uint Channel
        {
            get
            {
                return Registers.RfChannelRegister.RF_CH;
            }
            set
            {
                if (Channel > Constants.MaxRfChannel)
                    throw new ArgumentOutOfRangeException(string.Format("Channel", "Channel should be 0 - {0}", Constants.MaxRfChannel));
                Registers.RfChannelRegister.RF_CH = (byte)value;
                Registers.RfChannelRegister.Save();
            }
        }

        public bool IsPlusModel;
        public RadioModels RadioModel => IsPlusModel ? RadioModels.nRF24L01Plus : RadioModels.nRF24L01;

        public bool CrcEnabled
        {
            get
            {
                return Registers.ConfigurationRegister.EN_CRC;
            }
            set
            {
                Registers.ConfigurationRegister.EN_CRC = value;
                Registers.ConfigurationRegister.Save();
            }
        }

        public CrcEncodingSchemes CrcEncodingScheme
        {
            get
            {
                return Registers.ConfigurationRegister.CRCO ? CrcEncodingSchemes.DualBytes : CrcEncodingSchemes.SingleByte;
            }
            set
            {
                Registers.ConfigurationRegister.CRCO = (value == CrcEncodingSchemes.DualBytes);
                Registers.ConfigurationRegister.Save();
            }
        }

        public AutoRetransmitDelays AutoRetransmitDelay
        {
            get
            {
                return (AutoRetransmitDelays)Registers.SetupRetransmissionRegister.ARD;
            }
            set
            {
                Registers.SetupRetransmissionRegister.ARD = (byte)value;
                Registers.SetupRetransmissionRegister.Save();
            }
        }

        public byte AutoRetransmitCount
        {
            get
            {
                return Registers.SetupRetransmissionRegister.ARC;
            }
            set
            {
                if (value > 15)
                    throw new ArgumentOutOfRangeException(nameof(AutoRetransmitCount), "AutoRetransmitCount should be 0 - 15");
                Registers.SetupRetransmissionRegister.ARC = value;
                Registers.SetupRetransmissionRegister.Save();
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
                byte[] result = _radio.Transfer(new[] { Commands.R_RX_PL_WID, Commands.NOP });
                return result[0];
            }
        }

        private byte _payloadSize;

        public byte PayloadSize
        {
            get { return _payloadSize; }
            set { _payloadSize = Math.Min(value, Constants.MaxPayloadWidth); }
        }

        public void ToggleFeatures()
        {
            _radio.Transfer(new byte[] { Commands.ACTIVATE, 0x73 });
        }

        public byte ReadRegister(byte register)
        {
            byte[] request = new byte[2],
                response = new byte[1];

            request[0] = (byte)(Commands.R_REGISTER | (Commands.REGISTER_MASK & register));
            response = _radio.Transfer(request);
            return response[0];
        }

        public byte ReadRegister(byte register, ref byte[] buffer, int length = 0)
        {
            byte[] request = new byte[length + 1],
             response = new byte[length];
            request[0] = (byte)(Commands.R_REGISTER | (Commands.REGISTER_MASK & register));
            Array.Copy(buffer, 0, request, 1, length);
            response = _radio.Transfer(request);
            buffer = new byte[response.Length - 1];
            Array.Copy(response, 0, buffer, 0, buffer.Length);
            return response[0];
        }

        public byte WriteRegister(byte register, byte value)
        {
            byte[] status = _radio.Transfer(new[] { (byte)(Commands.W_REGISTER | (Commands.REGISTER_MASK & register)), value });
            _radio.Transfer(value);
            return status[0];
        }

        public byte WriteRegister(byte register, byte[] values, int length)
        {
            byte[] buffer = new byte[length + 1];
            buffer[0] = (byte)(Commands.W_REGISTER | (Commands.REGISTER_MASK & register));
            Array.Copy(values, 0, buffer, 1, length);
            byte[] status = _radio.Transfer(buffer);
            return status[0];
        }

        private string FormatObserveTx(byte value)
        {
            return string.Format("OBSERVE_TX={0:X2}: POLS_CNT={1} ARC_CNT={2}",
                value,
                (value >> Properties.PLOS_CNT) & 0xF,
                (value >> Properties.ARC_CNT) & 0xF);
        }

        public byte GetStatus()
        {
            return ReadRegister(nRF24L01.Registers.Addresses.STATUS);
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
                (status & Properties.RX_DR) > 0,
                (status & Properties.TX_DS) > 0,
                (status & Properties.MAX_RT) > 0,
                (status << (4) >> (5)),
                (status & Properties.TX_FULL) > 0);

            sb.AppendLine(GetAddressRegister("RX_ADDR_P0-1", nRF24L01.Registers.Addresses.RX_ADDR_P0, 2));
            sb.AppendLine(GetByteRegister("RX_ADDR_P2-5", nRF24L01.Registers.Addresses.RX_ADDR_P2, 4));
            sb.AppendLine(GetAddressRegister("TX_ADDR", nRF24L01.Registers.Addresses.TX_ADDR, 1));

            sb.AppendLine(GetByteRegister("RX_PW_P0-6", nRF24L01.Registers.Addresses.RX_PW_P0, 6));
            sb.AppendLine(GetByteRegister("EN_AA", nRF24L01.Registers.Addresses.EN_AA, 1));
            sb.AppendLine(GetByteRegister("EN_RXADDR", nRF24L01.Registers.Addresses.EN_RXADDR, 1));
            sb.AppendLine(GetByteRegister("RF_CH", nRF24L01.Registers.Addresses.RF_CH, 1));
            sb.AppendLine(GetByteRegister("RF_SETUP", nRF24L01.Registers.Addresses.RF_SETUP, 1));
            sb.AppendLine(GetByteRegister("CONFIG", nRF24L01.Registers.Addresses.CONFIG, 1));
            sb.AppendLine(GetByteRegister("DYNPD/FEATURE", nRF24L01.Registers.Addresses.DYNPD, 2));

            sb.AppendLine("Data Rate = " + Constants.DataRates[(int)DataRate]);
            sb.AppendLine("Model = " + _radio.Name);
            sb.AppendLine("CRC Enabled = " + CrcEnabled);
            sb.AppendLine("CRC Encoding Scheme = " + Constants.CrcEncodingSchemes[(int)CrcEncodingScheme]);
            sb.AppendLine("PA Power = " + Constants.PowerLevels[(int)PowerLevel]);

            sb.AppendFormat("\r\n{0}\r\n", Registers);
            return sb.ToString();
        }

        public override string ToString()
        {
            return Registers.ToString();
        }
    }
}
