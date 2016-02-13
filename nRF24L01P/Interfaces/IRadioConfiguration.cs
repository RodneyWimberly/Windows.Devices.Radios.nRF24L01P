using Windows.Devices.Radios.nRF24L01P.Enums;
using Windows.Devices.Radios.nRF24L01P.Registers;

namespace Windows.Devices.Radios.nRF24L01P.Interfaces
{
    public interface IRadioConfiguration
    {
        bool IsPlusModel { get; set; }
        IRegisterManager Registers { get; }
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