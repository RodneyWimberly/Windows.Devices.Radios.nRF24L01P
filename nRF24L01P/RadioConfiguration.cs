using Newtonsoft.Json;
using System;
using Windows.Devices.Radios.nRF24L01P.Enums;
using Windows.Devices.Radios.nRF24L01P.Interfaces;
using Windows.Devices.Radios.nRF24L01P.Registers;

namespace Windows.Devices.Radios.nRF24L01P
{
    public class RadioConfiguration : IRadioConfiguration
    {
        private readonly ICommandProcessor _commandProcessor;

        private readonly IRegisterManager _registerManager;

        public RadioConfiguration(ICommandProcessor commandProcessor, IRegisterManager registerManager)
        {
            _commandProcessor = commandProcessor;
            _registerManager = registerManager;
            _payloadWidth = Constants.MaxPayloadWidth;

            // Attempt to set DataRate to 250Kbps to determine if this is a plus model
            DataRates oldDataRate = DataRate;
            DataRate = DataRates.DataRate250Kbps;
            IsPlusModel = DataRate == DataRates.DataRate250Kbps;
            DataRate = oldDataRate;

            _commandProcessor.ControllerName = RadioModel.GetName();
        }

        public bool EnableContiuousCarrierTransmit
        {
            get
            {
                RfSetupRegister rfSetupRegister = _registerManager.RfSetupRegister;
                rfSetupRegister.Load();
                return rfSetupRegister.EnableContiuousCarrierTransmit && rfSetupRegister.ForcePllSignalLock;
            }
            set
            {
                RfSetupRegister rfSetupRegister = _registerManager.RfSetupRegister;
                rfSetupRegister.EnableContiuousCarrierTransmit = value;
                rfSetupRegister.ForcePllSignalLock = value;
                rfSetupRegister.Save();
            }
        }

        public bool EnableAutoAcknowledgement
        {
            get
            {
                EnableAutoAcknowledgementRegister autoAckRegister = _registerManager.EnableAutoAcknowledgementRegister;
                autoAckRegister.Load();
                return autoAckRegister.EnableAutoAcknowledgement;
            }
            set
            {
                EnableAutoAcknowledgementRegister autoAckRegister = _registerManager.EnableAutoAcknowledgementRegister;
                autoAckRegister.EnableAutoAcknowledgement = value;
                autoAckRegister.Save();
            }
        }

        public DataRates DataRate
        {
            get
            {
                DataRates dataRate = DataRates.DataRate1Mbps;
                RfSetupRegister rfSetupRegister = _registerManager.RfSetupRegister;
                rfSetupRegister.Load();
                bool low = rfSetupRegister.DataRateLow;
                bool high = rfSetupRegister.DataRateHigh;
                if (low)
                    dataRate = DataRates.DataRate250Kbps;
                else if (high)
                    dataRate = DataRates.DataRate2Mbps;

                return dataRate;
            }
            set
            {
                RfSetupRegister rfSetupRegister = _registerManager.RfSetupRegister;
                switch (value)
                {
                    case DataRates.DataRate1Mbps:
                        rfSetupRegister.DataRateLow = false;
                        rfSetupRegister.DataRateHigh = false;
                        break;
                    case DataRates.DataRate2Mbps:
                        rfSetupRegister.DataRateLow = false;
                        rfSetupRegister.DataRateHigh = true;
                        break;
                    case DataRates.DataRate250Kbps:
                        rfSetupRegister.DataRateLow = true;
                        rfSetupRegister.DataRateHigh = false;
                        break;
                    default:
                        throw new InvalidOperationException("Invalid data rate value");
                }
                _registerManager.RfSetupRegister.Save();
            }
        }

        public PowerLevels PowerLevel
        {
            get
            {
                PowerLevels powerLevel = PowerLevels.Error;
                RfSetupRegister rfSetupRegister = _registerManager.RfSetupRegister;
                rfSetupRegister.Load();
                bool low = rfSetupRegister.PowerLevelLow;
                bool high = rfSetupRegister.PowerLevelHigh;
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
                RfSetupRegister rfSetupRegister = _registerManager.RfSetupRegister;
                switch (value)
                {
                    case PowerLevels.Max:
                        rfSetupRegister.PowerLevelLow = true;
                        rfSetupRegister.PowerLevelHigh = true;
                        break;
                    case PowerLevels.High:
                        rfSetupRegister.PowerLevelLow = false;
                        rfSetupRegister.PowerLevelHigh = true;
                        break;
                    case PowerLevels.Low:
                        rfSetupRegister.PowerLevelLow = true;
                        rfSetupRegister.PowerLevelHigh = false;
                        break;
                    case PowerLevels.Min:
                        rfSetupRegister.PowerLevelLow = false;
                        rfSetupRegister.PowerLevelHigh = false;
                        break;
                }
                rfSetupRegister.Save();
            }
        }

        public uint Channel
        {
            get
            {
                _registerManager.RfChannelRegister.Load();
                return _registerManager.RfChannelRegister.RfChannel;
            }
            set
            {
                if (Channel > Constants.MaxRfChannel)
                    throw new ArgumentOutOfRangeException(string.Format("Channel", "Channel should be 0 - {0}", Constants.MaxRfChannel));
                _registerManager.RfChannelRegister.RfChannel = (byte)value;
                _registerManager.RfChannelRegister.Save();
            }
        }

        public bool IsPlusModel { get; set; }
        public RadioModels RadioModel => IsPlusModel ? RadioModels.nRF24L01Plus : RadioModels.nRF24L01;

        public bool CrcEnabled
        {
            get
            {
                _registerManager.ConfigurationRegister.Load();
                return _registerManager.ConfigurationRegister.EnableCrc;
            }
            set
            {
                _registerManager.ConfigurationRegister.EnableCrc = value;
                _registerManager.ConfigurationRegister.Save();
            }
        }

        public CrcEncodingSchemes CrcEncodingScheme
        {
            get
            {
                _registerManager.ConfigurationRegister.Load();
                return _registerManager.ConfigurationRegister.CrcEncodingScheme ? CrcEncodingSchemes.DualBytes : CrcEncodingSchemes.SingleByte;
            }
            set
            {
                _registerManager.ConfigurationRegister.CrcEncodingScheme = (value == CrcEncodingSchemes.DualBytes);
                _registerManager.ConfigurationRegister.Save();
            }
        }

        public AutoRetransmitDelays AutoRetransmitDelay
        {
            get
            {
                _registerManager.SetupRetransmissionRegister.Load();
                return (AutoRetransmitDelays)_registerManager.SetupRetransmissionRegister.AutoRetransmitDelay;
            }
            set
            {
                _registerManager.SetupRetransmissionRegister.AutoRetransmitDelay = (byte)value;
                _registerManager.SetupRetransmissionRegister.Save();
            }
        }

        public byte AutoRetransmitCount
        {
            get
            {
                _registerManager.SetupRetransmissionRegister.Load();
                return _registerManager.SetupRetransmissionRegister.AutoRetransmitCount;
            }
            set
            {
                if (value > 15)
                    throw new ArgumentOutOfRangeException(nameof(AutoRetransmitCount), "AutoRetransmitCount should be 0 - 15");
                _registerManager.SetupRetransmissionRegister.AutoRetransmitCount = value;
                _registerManager.SetupRetransmissionRegister.Save();
            }
        }

        public int AddressWidth
        {
            get
            {
                _registerManager.AddressWidthRegister.Load();
                switch ((int)_registerManager.AddressWidthRegister.AddressWidth)
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
                        _registerManager.AddressWidthRegister.AddressWidth = (byte)1;
                        break;
                    case 4:
                        _registerManager.AddressWidthRegister.AddressWidth = (byte)2;
                        break;
                    case 5:
                        _registerManager.AddressWidthRegister.AddressWidth = (byte)3;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(value), "AddressWidth should be 3,4,5 only");
                }
                _registerManager.AddressWidthRegister.Save();
            }
        }

        public bool DynamicPayloadLengthEnabled
        {
            get
            {
                _registerManager.FeatureRegister.Load();
                return _registerManager.FeatureRegister.EnableDynamicPayloadLength;
            }
            set
            {
                _registerManager.FeatureRegister.EnableDynamicPayloadLength = value;
                _registerManager.FeatureRegister.Save();
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
