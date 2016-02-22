namespace Windows.Devices.Radios.nRF24L01P.Registers
{
    internal static class RegisterDefaults
    {
        /// <summary>
        ///   Configuration Register
        /// </summary>
        public static readonly byte[] CONFIG = { 0x08 };

        /// <summary>
        ///   Enable 'Auto Acknowledgment' Function. Disable this functionality to be compatible with nRF2401.
        /// </summary>
        public static readonly byte[] EN_AA = { 0x3F };

        /// <summary>
        ///   Enabled RX Addresses
        /// </summary>
        public static readonly byte[] EN_RXADDR = { 0x03 };

        /// <summary>
        ///   Setup of Address Widths (common for all data pipes)
        /// </summary>
        public static readonly byte[] SETUP_AW = { 0x03 };

        /// <summary>
        ///   Setup of Automatic Retransmission
        /// </summary>
        public static readonly byte[] SETUP_RETR = { 0x03 };

        /// <summary>
        ///   RF Channel
        /// </summary>
        public static readonly byte[] RF_CH = { 0x02 };

        /// <summary>
        ///   RF Setup Register
        /// </summary>
        public static readonly byte[] RF_SETUP = { 0x0E };

        /// <summary>
        ///   Status Register (In parallel to the SPI command word applied on the MOSI pin, the STATUS register is shifted serially out on the MISO pin)
        /// </summary>
        public static readonly byte[] STATUS = { 0x0E };

        /// <summary>
        ///   Transmit observe register
        /// </summary>
        public static readonly byte[] OBSERVE_TX = { 0x00 };

        /// <summary>
        ///   Received Power Detector.
        /// </summary>
        public static readonly byte[] RPD = { 0x00 };

        /// <summary>
        ///   Receive address data pipe 0. 5 Bytes maximum length. (LSByte is written first. Write the number of bytes defined by SETUP_AW)
        /// </summary>
        public static readonly byte[] RX_ADDR_P0 = { 0xE7, 0xE7, 0xE7, 0xE7, 0xE7 };

        /// <summary>
        ///   Receive address data pipe 1. 5 Bytes maximum length. (LSByte is written first. Write the number of bytes defined by SETUP_AW)
        /// </summary>
        public static readonly byte[] RX_ADDR_P1 = { 0xC2, 0xC2, 0xC2, 0xC2, 0xC2 };

        /// <summary>
        ///   Receive address data pipe 2. Only LSB. MSBytes are equal to RX_ADDR_P1
        /// </summary>
        public static readonly byte[] RX_ADDR_P2 = { 0xC3 };

        /// <summary>
        ///   Receive address data pipe 3. Only LSB. MSBytes are equal to RX_ADDR_P1
        /// </summary>
        public static readonly byte[] RX_ADDR_P3 = { 0xC4 };

        /// <summary>
        ///   Receive address data pipe 4. Only LSB. MSBytes are equal to RX_ADDR_P1
        /// </summary>
        public static readonly byte[] RX_ADDR_P4 = { 0xC5 };

        /// <summary>
        ///   Receive address data pipe 5. Only LSB. MSBytes are equal to RX_ADDR_P1
        /// </summary>
        public static readonly byte[] RX_ADDR_P5 = { 0xC6 };

        /// <summary>
        ///   Transmit address. Used for a PTX device only. (LSByte is written first) 
        ///   Set RX_ADDR_P0 equal to this address to handle automatic acknowledge if this is a PTX device with Enhanced ShockBurst™ enabled.
        /// </summary>
        public static readonly byte[] TX_ADDR = { 0xC2, 0xC2, 0xC2, 0xC2, 0xC2 };

        /// <summary>
        ///   Number of bytes in RX payload in data pipe 0
        /// </summary>
        public static readonly byte[] RX_PW_P0 = { 0x00 };

        /// <summary>
        ///   Number of bytes in RX payload in data pipe 1
        /// </summary>
        public static readonly byte[] RX_PW_P1 = { 0x00 };

        /// <summary>
        ///   Number of bytes in RX payload in data pipe 2
        /// </summary>
        public static readonly byte[] RX_PW_P2 = { 0x00 };

        /// <summary>
        ///   Number of bytes in RX payload in data pipe 3
        /// </summary>
        public static readonly byte[] RX_PW_P3 = { 0x00 };

        /// <summary>
        ///   Number of bytes in RX payload in data pipe 4
        /// </summary>
        public static readonly byte[] RX_PW_P4 = { 0x00 };

        /// <summary>
        ///   Number of bytes in RX payload in data pipe 5
        /// </summary>
        public static readonly byte[] RX_PW_P5 = { 0x00 };

        /// <summary>
        ///   FIFO Status Register
        /// </summary>
        public static readonly byte[] FIFO_STATUS = { 0x11 };

        /// <summary>
        ///   Enable dynamic payload length
        /// </summary>
        public static readonly byte[] DYNPD = { 0x00 };

        /// <summary>
        ///   Feature Register
        /// </summary>
        public static readonly byte[] FEATURE = { 0x00 };

    }
}
