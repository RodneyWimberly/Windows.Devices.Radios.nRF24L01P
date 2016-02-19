using Windows.Devices.Radios.nRF24L01P.Enums;

namespace Windows.Devices.Radios.nRF24L01P.Interfaces
{
    public interface IRadioConfiguration
    {
        bool EnableContiuousCarrierTransmit { get; set; }
        bool IsPlusModel { get; set; }
        bool EnableAutoAcknowledgement { get; set; }
        DataRates DataRate { get; set; }
        PowerLevels PowerLevel { get; set; }
        uint Channel { get; set; }
        RadioModels RadioModel { get; }
        bool CrcEnabled { get; set; }
        CrcEncodingSchemes CrcEncodingScheme { get; set; }
        AutoRetransmitDelays AutoRetransmitDelay { get; set; }
        byte AutoRetransmitCount { get; set; }
        int AddressWidth { get; set; }
        bool DynamicPayloadLengthEnabled { get; set; }
        byte DynamicPayloadSize { get; }
        byte PayloadWidth { get; set; }
        void ToggleFeatures();
        string ToString();
    }
}