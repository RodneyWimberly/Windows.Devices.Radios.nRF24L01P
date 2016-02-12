using Windows.Devices.Radios.nRF24L01P.Enums;

namespace Windows.Devices.Radios.nRF24L01P
{
    public static class ExtensionMethods
    {
        private static readonly string[] _dataRates;
        private static readonly string[] _crcEncodingSchemes;
        private static readonly string[] _powerLevels;
        private static readonly string[] _radioModels;

        static ExtensionMethods()
        {
            _dataRates = new [] { "1 Mbps", "2 Mbps", "250 Kbps" };
            _crcEncodingSchemes = new[] { "Single Byte", "Double Byte" };
            _powerLevels = new[] { "PA_MIN", "PA_LOW", "PA_HIGH", "PA_MAX", "PA_ERROR" };
            _radioModels = new[] { "nRF24L01", "nRF24L01+" };
        }

        public static string GetName(this DataRates dataRate)
        {
            return _dataRates[(int)dataRate];
        }

        public static string GetName(this CrcEncodingSchemes crcEncodingScheme)
        {
            return _crcEncodingSchemes[(int)crcEncodingScheme];
        }

        public static string GetName(this PowerLevels powerLevel)
        {
            return _powerLevels[(int) powerLevel];
        }

        public static string GetName(this RadioModels radioModel)
        {
            return _radioModels[(int)radioModel];
        }
    }
}
