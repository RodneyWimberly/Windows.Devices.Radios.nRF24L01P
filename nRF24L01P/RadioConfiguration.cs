using Newtonsoft.Json;
using System;
using Windows.Devices.Radios.nRF24L01P.Enums;
using Windows.Devices.Radios.nRF24L01P.Registers;

namespace Windows.Devices.Radios.nRF24L01P
{
    public class RadioConfiguration
    {
        private readonly ICommandProcessor _commandProcessor;

        public RegisterManager Registers { get; private set; }

        public RadioConfiguration(ICommandProcessor commandProcessor)
        {
            _commandProcessor = commandProcessor;
            _payloadWidth = Constants.MaxPayloadWidth;
            Registers = new RegisterManager(_commandProcessor);
            Registers.LoadRegisters();
        }

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
                bool low = Registers.RfSetupRegister.RF_PWR_LOW;
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
                        throw new ArgumentOutOfRangeException(nameof(value), "AddressWidth should be 3,4,5 only");
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

        public byte DynamicPayloadSize => _commandProcessor.ExecuteCommand(DeviceCommands.R_RX_PL_WID, RegisterAddresses.EMPTY_ADDRESS, new byte[1])[0];

        private byte _payloadWidth;

        public byte PayloadWidth
        {
            get { return _payloadWidth; }
            set { _payloadWidth = Math.Min(value, Constants.MaxPayloadWidth); }
        }

        public void ToggleFeatures()
        {
            _commandProcessor.ExecuteCommand(DeviceCommands.ACTIVATE);
            _commandProcessor.ExecuteCommand(DeviceCommands.FEATURES);
        }

        public override string ToString()
        {
            return GetType().Name + ":\r\n" + JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}
