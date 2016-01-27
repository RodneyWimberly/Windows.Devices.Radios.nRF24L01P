namespace Windows.Devices.Radios.nRF24L01
{
    internal static class Constants
    {
        internal static string[] DataRateStrings = { "1 Mbps", "2 Mbps", "250 Kbps" };
        internal static string[] CrcLengthStrings = { "Disabled", "8 Bits", "16 Bits" };
        internal static string[] PowerLevelStrings = { "PA_MIN", "PA_LOW", "PA_HIGH", "PA_MAX", "PA_ERROR" };
        internal static string[] RadioModelStrings = { "nRF24L01", "nRF24L01+" };

        internal static byte[] ChildPipes = { RX_ADDR_P0, RX_ADDR_P1, RX_ADDR_P2, RX_ADDR_P3, RX_ADDR_P4, RX_ADDR_P5 };
        internal static byte[] ChildPayloadSizes = { RX_PW_P0, RX_PW_P1, RX_PW_P2, RX_PW_P3, RX_PW_P4, RX_PW_P5 };
        internal static byte[] ChildPipeEnable = { ERX_P0, ERX_P1, ERX_P2, ERX_P3, ERX_P4, ERX_P5 };

        internal const byte MaxChannel = 127;
        internal const byte MaxPayloadSize = 32;

        //CONFIG register bitwise definitions
        internal const byte CONFIG_RESERVED = 0x80;
        internal const byte CONFIG_MASK_RX_DR = 0x40;
        internal const byte CONFIG_MASK_TX_DS = 0x20;
        internal const byte CONFIG_MASK_MAX_RT = 0x10;
        internal const byte CONFIG_EN_CRC = 0x08;
        internal const byte CONFIG_CRCO = 0x04;
        internal const byte CONFIG_PWR_UP = 0x02;
        internal const byte CONFIG_PRIM_RX = 0x01;

        internal const byte STATUS_DEFAULT_VAL = 0x0E;
        internal const byte STATUS_RX_DR = 0x40;

        /* Memory Map */
        internal const byte CONFIG = 0x00;
        internal const byte EN_AA = 0x01;
        internal const byte EN_RXADDR = 0x02;
        internal const byte SETUP_AW = 0x03;
        internal const byte SETUP_RETR = 0x04;
        internal const byte RF_CH = 0x05;
        internal const byte RF_SETUP = 0x06;
        internal const byte STATUS = 0x07;
        internal const byte OBSERVE_TX = 0x08;
        internal const byte CD = 0x09;
        internal const byte RX_ADDR_P0 = 0x0A;
        internal const byte RX_ADDR_P1 = 0x0B;
        internal const byte RX_ADDR_P2 = 0x0C;
        internal const byte RX_ADDR_P3 = 0x0D;
        internal const byte RX_ADDR_P4 = 0x0E;
        internal const byte RX_ADDR_P5 = 0x0F;
        internal const byte TX_ADDR = 0x10;
        internal const byte RX_PW_P0 = 0x11;
        internal const byte RX_PW_P1 = 0x12;
        internal const byte RX_PW_P2 = 0x13;
        internal const byte RX_PW_P3 = 0x14;
        internal const byte RX_PW_P4 = 0x15;
        internal const byte RX_PW_P5 = 0x16;
        internal const byte FIFO_STATUS = 0x17;
        internal const byte DYNPD = 0x1C;
        internal const byte FEATURE = 0x1D;

        /* Bit Mnemonics */
        internal const byte MASK_RX_DR = 6;
        internal const byte MASK_TX_DS = 5;
        internal const byte MASK_MAX_RT = 4;
        internal const byte EN_CRC = 3;
        internal const byte CRCO = 2;
        internal const byte PWR_UP = 1;
        internal const byte PRIM_RX = 0;
        internal const byte ENAA_P5 = 5;
        internal const byte ENAA_P4 = 4;
        internal const byte ENAA_P3 = 3;
        internal const byte ENAA_P2 = 2;
        internal const byte ENAA_P1 = 1;
        internal const byte ENAA_P0 = 0;
        internal const byte ERX_P5 = 5;
        internal const byte ERX_P4 = 4;
        internal const byte ERX_P3 = 3;
        internal const byte ERX_P2 = 2;
        internal const byte ERX_P1 = 1;
        internal const byte ERX_P0 = 0;
        internal const byte AW = 0;
        internal const byte ARD = 4;
        internal const byte ARC = 0;
        internal const byte PLL_LOCK = 4;
        internal const byte RF_DR = 3;
        internal const byte RF_PWR = 6;
        internal const byte RX_DR = 6;
        internal const byte TX_DS = 5;
        internal const byte MAX_RT = 4;
        internal const byte RX_P_NO = 1;
        internal const byte TX_FULL = 0;
        internal const byte PLOS_CNT = 4;
        internal const byte ARC_CNT = 0;
        internal const byte TX_REUSE = 6;
        internal const byte FIFO_FULL = 5;
        internal const byte TX_EMPTY = 4;
        internal const byte RX_FULL = 1;
        internal const byte RX_EMPTY = 0;
        internal const byte DPL_P5 = 5;
        internal const byte DPL_P4 = 4;
        internal const byte DPL_P3 = 3;
        internal const byte DPL_P2 = 2;
        internal const byte DPL_P1 = 1;
        internal const byte DPL_P0 = 0;
        internal const byte EN_DPL = 2;
        internal const byte EN_ACK_PAY = 1;
        internal const byte EN_DYN_ACK = 0;

        /* Instruction Mnemonics */
        internal const byte R_REGISTER = 0x00;
        internal const byte W_REGISTER = 0x20;
        internal const byte REGISTER_MASK = 0x1F;
        internal const byte ACTIVATE = 0x50;
        internal const byte R_RX_PL_WID = 0x60;
        internal const byte R_RX_PAYLOAD = 0x61;
        internal const byte W_TX_PAYLOAD = 0xA0;
        internal const byte W_ACK_PAYLOAD = 0xA8;
        internal const byte FLUSH_TX = 0xE1;
        internal const byte FLUSH_RX = 0xE2;
        internal const byte REUSE_TX_PL = 0xE3;
        internal const byte NOP = 0xFF;
        internal const byte W_TX_PAYLOAD_NOACK = 0xB0;

        /* Non-P omissions */
        internal const byte LNA_HCURR = 0;

        /* P model memory Map */
        internal const byte RPD = 0x09;

        /* P model bit Mnemonics */
        internal const byte RF_DR_LOW = 5;
        internal const byte RF_DR_HIGH = 3;
        internal const byte RF_PWR_LOW = 1;
        internal const byte RF_PWR_HIGH = 2;
    }
}
