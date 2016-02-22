using Windows.Devices.Radios.nRF24L01P.Enums;

namespace Windows.Devices.Radios.nRF24L01P.Interfaces
{
    public interface IConfiguration
    {
        bool EnableContiuousCarrierTransmit { get; set; }
        bool EnableAutoAcknowledgement { get; set; }
        DataRates DataRate { get; set; }
        string DateRateName { get; }
        PowerLevels PowerLevel { get; set; }
        string PowerLevelName { get; }
        uint Channel { get; set; }
        RadioModels RadioModel { get; }
        string RadioModelName { get; }
        CrcEncodingSchemes CrcEncodingScheme { get; set; }
        string CrcEncodingSchemeName { get; }
        AutoRetransmitDelays AutoRetransmitDelay { get; set; }
        string AutoRetransmitDelayName { get; }
        byte AutoRetransmitCount { get; set; }
        int AddressWidth { get; set; }
        bool DynamicPayloadLengthEnabled { get; set; }
        byte DynamicPayloadSize { get; }
        byte PayloadWidth { get; set; }
        void ToggleFeatures();
        string ToString();
    }
}