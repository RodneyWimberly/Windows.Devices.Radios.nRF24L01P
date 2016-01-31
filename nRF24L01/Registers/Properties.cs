namespace Windows.Devices.Radios.nRF24L01.Registers
{
    internal static class Properties
    {
        // Configuration Register
        internal const byte MASK_RX_DR = 6;
        internal const byte MASK_TX_DS = 5;
        internal const byte MASK_MAX_RT = 4;
        internal const byte EN_CRC = 3;
        internal const byte CRCO = 2;
        internal const byte PWR_UP = 1;
        internal const byte PRIM_RX = 0;

        // Dynamic Payload Length Register
        internal const byte DPL_P5 = 5;
        internal const byte DPL_P4 = 4;
        internal const byte DPL_P3 = 3;
        internal const byte DPL_P2 = 2;
        internal const byte DPL_P1 = 1;
        internal const byte DPL_P0 = 0;

        // Enable Auto Acknowledgement Register 
        internal const byte ENAA_P5 = 5;
        internal const byte ENAA_P4 = 4;
        internal const byte ENAA_P3 = 3;
        internal const byte ENAA_P2 = 2;
        internal const byte ENAA_P1 = 1;
        internal const byte ENAA_P0 = 0;

        // Enable Receive Address Register
        internal static byte[] ReceivePipeEnable = { ERX_P0, ERX_P1, ERX_P2, ERX_P3, ERX_P4, ERX_P5 };
        internal const byte ERX_P5 = 5;
        internal const byte ERX_P4 = 4;
        internal const byte ERX_P3 = 3;
        internal const byte ERX_P2 = 2;
        internal const byte ERX_P1 = 1;
        internal const byte ERX_P0 = 0;

        // Feature Register
        internal const byte EN_DPL = 2;
        internal const byte EN_ACK_PAY = 1;
        internal const byte EN_DYN_ACK = 0;

        // FIFO Status Register
        internal const byte TX_REUSE = 6;
        internal const byte FIFO_FULL = 5;
        internal const byte TX_EMPTY = 4;
        internal const byte RX_FULL = 1;
        internal const byte RX_EMPTY = 0;

        // Observe Transmit Register
        internal const byte PLOS_CNT = 4; // (7 bits wide)
        internal const byte ARC_CNT = 0; // (3 bits wide)

        // Received Power Detector Register
        internal const byte RPD = 0;

        // Receive Payload Width Pipe 0 Register
        internal const byte RX_PW_P0 = 0; // (5 bits wide)

        // Receive Payload Width Pipe 1 Register
        internal const byte RX_PW_P1 = 0; // (5 bits wide)

        // Receive Payload Width Pipe 2 Register
        internal const byte RX_PW_P2 = 0; // (5 bits wide)

        // Receive Payload Width Pipe 3 Register
        internal const byte RX_PW_P3 = 0; // (5 bits wide)

        // Receive Payload Width Pipe 4 Register
        internal const byte RX_PW_P4 = 0; // (5 bits wide)

        // Receive Payload Width Pipe 5 Register
        internal const byte RX_PW_P5 = 0; // (5 bits wide)

        // RF Channel Register
        internal const byte RF_CH = 0; // (6 bits wide)

        // RF Setup register
        internal const byte CONT_WAVE = 6; //0x80;
        internal const byte RF_DR_LOW = 5;
        internal const byte PLL_LOCK = 4;
        internal const byte RF_DR_HIGH = 3;
        internal const byte RF_PWR = 1; // (2 bits wide)

        // Setup Address Width Register
        internal const byte AW = 1;

        // Setup Retransmission Register
        internal const byte ARD = 4; // (7 bits wide)
        internal const byte ARC = 0; // (3 bits wide)

        // Status Register
        internal const byte RX_DR = 6;
        internal const byte TX_DS = 5;
        internal const byte MAX_RT = 4;
        internal const byte RX_P_NO = 1; // (3 bit wide)
        internal const byte TX_FULL = 0;
    }
}