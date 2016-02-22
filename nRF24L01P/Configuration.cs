using Newtonsoft.Json;
using System;
using Windows.Devices.Radios.nRF24L01P.Enums;
using Windows.Devices.Radios.nRF24L01P.Interfaces;
using Windows.Devices.Radios.nRF24L01P.Registers;

namespace Windows.Devices.Radios.nRF24L01P
{
    public class Configuration : IConfiguration
    {
        private readonly ICommandProcessor _commandProcessor;
        private readonly IRegisterContainer _registerContainer;
        private readonly bool _isPlusModel;

        public Configuration(ICommandProcessor commandProcessor, IRegisterContainer registerContainer)
        {
            _commandProcessor = commandProcessor;
            _registerContainer = registerContainer;
            _payloadWidth = Constants.MaxPayloadWidth;

            // Attempt to set DataRate to 250Kbps to determine if this is a plus model
            DataRates oldDataRate = DataRate;
            DataRate = DataRates.DataRate250Kbps;
            _isPlusModel = DataRate == DataRates.DataRate250Kbps;
            DataRate = oldDataRate;

            _commandProcessor.ControllerName = RadioModel.GetName();
        }

        public bool EnableContiuousCarrierTransmit
        {
            get
            {
                RfSetupRegister rfSetupRegister = _registerContainer.RfSetupRegister;
                rfSetupRegister.Load();
                return rfSetupRegister.EnableContiuousCarrierTransmit && rfSetupRegister.ForcePllSignalLock;
            }
            set
            {
                RfSetupRegister rfSetupRegister = _registerContainer.RfSetupRegister;
                rfSetupRegister.EnableContiuousCarrierTransmit = value;
                rfSetupRegister.ForcePllSignalLock = value;
                rfSetupRegister.Save();
            }
        }

        public bool EnableAutoAcknowledgement
        {
            get
            {
                EnableAutoAcknowledgementRegister autoAckRegister = _registerContainer.EnableAutoAcknowledgementRegister;
                autoAckRegister.Load();
                return autoAckRegister.EnableAutoAcknowledgement;
            }
            set
            {
                EnableAutoAcknowledgementRegister autoAckRegister = _registerContainer.EnableAutoAcknowledgementRegister;
                autoAckRegister.EnableAutoAcknowledgement = value;
                autoAckRegister.Save();
            }
        }

        public string DateRateName => DataRate.GetName();

        public DataRates DataRate
        {
            get
            {
                DataRates dataRate = DataRates.DataRate1Mbps;
                RfSetupRegister rfSetupRegister = _registerContainer.RfSetupRegister;
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
                RfSetupRegister rfSetupRegister = _registerContainer.RfSetupRegister;
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
                _registerContainer.RfSetupRegister.Save();
            }
        }

        public string PowerLevelName => PowerLevel.GetName();

        public PowerLevels PowerLevel
        {
            get
            {
                PowerLevels powerLevel;
                RfSetupRegister rfSetupRegister = _registerContainer.RfSetupRegister;
                rfSetupRegister.Load();
                if (!Enum.TryParse(rfSetupRegister.PowerLevel.ToString(), out powerLevel))
                    powerLevel = PowerLevels.Error;
                return powerLevel;
            }
            set
            {
                RfSetupRegister rfSetupRegister = _registerContainer.RfSetupRegister;
                rfSetupRegister.PowerLevel = (byte)value;
                rfSetupRegister.Save();
            }
        }

        public uint Channel
        {
            get
            {
                _registerContainer.RfChannelRegister.Load();
                return _registerContainer.RfChannelRegister.RfChannel;
            }
            set
            {
                if (Channel > Constants.MaxRfChannel)
                    throw new ArgumentOutOfRangeException(string.Format("Channel", "Channel should be 0 - {0}", Constants.MaxRfChannel));
                _registerContainer.RfChannelRegister.RfChannel = (byte)value;
                _registerContainer.RfChannelRegister.Save();
            }
        }


        public string RadioModelName => RadioModel.GetName();
        public RadioModels RadioModel => _isPlusModel ? RadioModels.nRF24L01Plus : RadioModels.nRF24L01;

        public string CrcEncodingSchemeName => CrcEncodingScheme.GetName();

        public CrcEncodingSchemes CrcEncodingScheme
        {
            get
            {
                CrcEncodingSchemes crcEncodingScheme = CrcEncodingSchemes.None;
                ConfigurationRegister configurationRegister = _registerContainer.ConfigurationRegister;
                configurationRegister.Load();
                if (configurationRegister.EnableCrc)
                    crcEncodingScheme = configurationRegister.CrcEncodingScheme ? CrcEncodingSchemes.DualBytes : CrcEncodingSchemes.SingleByte;
                return crcEncodingScheme;
            }
            set
            {
                ConfigurationRegister configurationRegister = _registerContainer.ConfigurationRegister;
                configurationRegister.EnableCrc = value != CrcEncodingSchemes.None;
                if (configurationRegister.EnableCrc)
                    configurationRegister.CrcEncodingScheme = (value == CrcEncodingSchemes.DualBytes);
                configurationRegister.Save();
            }
        }

        public string AutoRetransmitDelayName => AutoRetransmitDelay.GetName();

        public AutoRetransmitDelays AutoRetransmitDelay
        {
            get
            {
                _registerContainer.SetupRetransmissionRegister.Load();
                return (AutoRetransmitDelays)_registerContainer.SetupRetransmissionRegister.AutoRetransmitDelay;
            }
            set
            {
                _registerContainer.SetupRetransmissionRegister.AutoRetransmitDelay = (byte)value;
                _registerContainer.SetupRetransmissionRegister.Save();
            }
        }

        public byte AutoRetransmitCount
        {
            get
            {
                _registerContainer.SetupRetransmissionRegister.Load();
                return _registerContainer.SetupRetransmissionRegister.AutoRetransmitCount;
            }
            set
            {
                if (value > 15)
                    throw new ArgumentOutOfRangeException(nameof(AutoRetransmitCount), "AutoRetransmitCount should be 0 - 15");
                _registerContainer.SetupRetransmissionRegister.AutoRetransmitCount = value;
                _registerContainer.SetupRetransmissionRegister.Save();
            }
        }

        public int AddressWidth
        {
            get
            {
                _registerContainer.AddressWidthRegister.Load();
                switch ((int)_registerContainer.AddressWidthRegister.AddressWidth)
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
                        _registerContainer.AddressWidthRegister.AddressWidth = (byte)1;
                        break;
                    case 4:
                        _registerContainer.AddressWidthRegister.AddressWidth = (byte)2;
                        break;
                    case 5:
                        _registerContainer.AddressWidthRegister.AddressWidth = (byte)3;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(value), "AddressWidth should be 3,4,5 only");
                }
                _registerContainer.AddressWidthRegister.Save();
            }
        }

        public bool DynamicPayloadLengthEnabled
        {
            get
            {
                _registerContainer.FeatureRegister.Load();
                _registerContainer.DynamicPayloadLengthRegister.Load();
                return _registerContainer.FeatureRegister.EnableDynamicPayloadLength &&
                    _registerContainer.DynamicPayloadLengthRegister.EnableDynamicPayload;
            }
            set
            {
                _registerContainer.DynamicPayloadLengthRegister.EnableDynamicPayload =
                    _registerContainer.FeatureRegister.EnableDynamicPayloadLength =
                    value;
                _registerContainer.DynamicPayloadLengthRegister.Save();
                _registerContainer.FeatureRegister.Save();
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
            return string.Format("{0}\r\n{1}\r\n", GetType().Name, JsonConvert.SerializeObject(this, Formatting.None));
        }
    }
}
