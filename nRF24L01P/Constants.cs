namespace Windows.Devices.Radios.nRF24L01P
{
    internal static class Constants
    {
        internal const byte MaxRfChannel = 127;
        internal const byte MaxPayloadWidth = 32;

        internal static string[] DataRates = { "1 Mbps", "2 Mbps", "250 Kbps" };
        internal static string[] CrcEncodingSchemes = { "Single Byte", "Double Byte" };
        internal static string[] PowerLevels = { "PA_MIN", "PA_LOW", "PA_HIGH", "PA_MAX", "PA_ERROR" };
        internal static string[] RadioModels = { "nRF24L01", "nRF24L01+" };
    }
}
