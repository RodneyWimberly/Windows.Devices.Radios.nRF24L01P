using Windows.Devices.Radios.nRF24L01P.Enums;

namespace Windows.Devices.Radios.nRF24L01P.Extensions
{
    public static class EnumExtensions
    {
        private static readonly string[] DataRates;
        private static readonly string[] CrcEncodingSchemes;
        private static readonly string[] PowerLevels;
        private static readonly string[] RadioModels;
        private static readonly string[] FifoStatus;
        private static readonly string[] OperatingMode;
        private static readonly string[] AutoRetransmitDelay;

        static EnumExtensions()
        {
            DataRates = new[] { "1 Mbps", "2 Mbps", "2.5 Mbps" };
            CrcEncodingSchemes = new[] { "None", "Single Byte", "Double Byte" };
            PowerLevels = new[] { "Minimum (-18dBm)", "Low (-12dBm)", "High (-6dBm)", "Maximum (0dBm)", "Error" };
            RadioModels = new[] { "nRF24L01", "nRF24L01+" };
            FifoStatus = new[] { "Full", "Empty", "In Use" };
            OperatingMode = new[] { "Power Off", "Power Down", "Stand-By", "Transmit Mode", "Receive Mode" };
            AutoRetransmitDelay = new[]
            {
                "250uS", "500uS", "750uS", "1000uS",
                "1250uS", "1500uS", "1750uS", "2000uS",
                "2250uS", "2500uS", "2750uS", "3000uS",
                "3250uS", "3500uS", "3750uS", "4000uS"
            };
        }

        public static string GetName(this DataRates dataRate)
        {
            return DataRates[(int)dataRate];
        }

        public static string GetName(this CrcEncodingSchemes crcEncodingScheme)
        {
            return CrcEncodingSchemes[(int)crcEncodingScheme];
        }

        public static string GetName(this PowerLevels powerLevel)
        {
            return PowerLevels[(int)powerLevel];
        }

        public static string GetName(this RadioModels radioModel)
        {
            return RadioModels[(int)radioModel];
        }

        public static string GetName(this FifoStatus fifoStatus)
        {
            return FifoStatus[(int)fifoStatus];
        }

        public static string GetName(this OperatingModes deviceStatus)
        {
            return OperatingMode[(int)deviceStatus];
        }

        public static string GetName(this AutoRetransmitDelays autoRetransmitDelay)
        {
            return AutoRetransmitDelay[(int)autoRetransmitDelay];
        }
    }
}
