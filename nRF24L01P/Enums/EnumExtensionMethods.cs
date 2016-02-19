namespace Windows.Devices.Radios.nRF24L01P.Enums
{
    public static class EnumExtensionMethods
    {
        private static readonly string[] _dataRates;
        private static readonly string[] _crcEncodingSchemes;
        private static readonly string[] _powerLevels;
        private static readonly string[] _radioModels;
        private static readonly string[] _fifoStatus;
        private static readonly string[] _deviceStatus;
        private static readonly string[] _autoRetransmitDelay;

        static EnumExtensionMethods()
        {
            _dataRates = new[] { "1 Mbps", "2 Mbps", "2.5 Mbps" };
            _crcEncodingSchemes = new[] { "Single Byte", "Double Byte" };
            _powerLevels = new[] { "Minimum (-18dBm)", "Low (-12dBm)", "High (-6dBm)", "Maximum (0dBm)", "Error" };
            _radioModels = new[] { "nRF24L01", "nRF24L01+" };
            _fifoStatus = new[] { "Full", "Empty", "In Use" };
            _deviceStatus = new[] { "Undefined", "Power Down", "Stand-By", "Transmit Mode", "Receive Mode" };
            _autoRetransmitDelay = new[]
            {
                "250uS", "500uS", "750uS", "1000uS",
                "1250uS", "1500uS", "1750uS", "2000uS",
                "2250uS", "2500uS", "2750uS", "3000uS",
                "3250uS", "3500uS", "3750uS", "4000uS"
            };
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
            return _powerLevels[(int)powerLevel];
        }

        public static string GetName(this RadioModels radioModel)
        {
            return _radioModels[(int)radioModel];
        }

        public static string GetName(this FifoStatus fifoStatus)
        {
            return _fifoStatus[(int)fifoStatus];
        }

        public static string GetName(this DeviceStatus deviceStatus)
        {
            return _deviceStatus[(int)deviceStatus];
        }

        public static string GetName(this AutoRetransmitDelays autoRetransmitDelay)
        {
            return _autoRetransmitDelay[(int)autoRetransmitDelay];
        }
    }
}
