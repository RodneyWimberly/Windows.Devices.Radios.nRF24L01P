using Windows.Devices.Radios.nRF24L01P.Enums;

namespace Windows.Devices.Radios.nRF24L01P.Registers
{
    internal static class PropertyMasks
    {
        // Configuration Register
        internal const BitMasks MASK_RX_DR = BitMasks.Bit6;
        internal const BitMasks MASK_TX_DS = BitMasks.Bit5;
        internal const BitMasks MASK_MAX_RT = BitMasks.Bit4;
        internal const BitMasks EN_CRC = BitMasks.Bit3;
        internal const BitMasks CRCO = BitMasks.Bit2;
        internal const BitMasks PWR_UP = BitMasks.Bit1;
        internal const BitMasks PRIM_RX = BitMasks.Bit0;

        // Dynamic Payload Length Register
        internal const BitMasks DPL_P5 = BitMasks.Bit5;
        internal const BitMasks DPL_P4 = BitMasks.Bit4;
        internal const BitMasks DPL_P3 = BitMasks.Bit3;
        internal const BitMasks DPL_P2 = BitMasks.Bit2;
        internal const BitMasks DPL_P1 = BitMasks.Bit1;
        internal const BitMasks DPL_P0 = BitMasks.Bit0;

        // Enable Auto Acknowledgement Register 
        internal const BitMasks ENAA_P5 = BitMasks.Bit5;
        internal const BitMasks ENAA_P4 = BitMasks.Bit4;
        internal const BitMasks ENAA_P3 = BitMasks.Bit3;
        internal const BitMasks ENAA_P2 = BitMasks.Bit2;
        internal const BitMasks ENAA_P1 = BitMasks.Bit1;
        internal const BitMasks ENAA_P0 = BitMasks.Bit0;

        // Enable Receive Address Register
        internal static BitMasks[] ReceivePipeEnable = { ERX_P0, ERX_P1, ERX_P2, ERX_P3, ERX_P4, ERX_P5 };
        internal const BitMasks ERX_P5 = BitMasks.Bit5;
        internal const BitMasks ERX_P4 = BitMasks.Bit4;
        internal const BitMasks ERX_P3 = BitMasks.Bit3;
        internal const BitMasks ERX_P2 = BitMasks.Bit2;
        internal const BitMasks ERX_P1 = BitMasks.Bit1;
        internal const BitMasks ERX_P0 = BitMasks.Bit0;

        // Feature Register
        internal const BitMasks EN_DPL = BitMasks.Bit2;
        internal const BitMasks EN_ACK_PAY = BitMasks.Bit1;
        internal const BitMasks EN_DYN_ACK = BitMasks.Bit0;

        // FIFO Status Register
        internal const BitMasks TX_REUSE = BitMasks.Bit6;
        internal const BitMasks FIFO_FULL = BitMasks.Bit5;
        internal const BitMasks TX_EMPTY = BitMasks.Bit4;
        internal const BitMasks RX_FULL = BitMasks.Bit1;
        internal const BitMasks RX_EMPTY = BitMasks.Bit0;

        // Observe Transmit Register
        internal const BitMasks PLOS_CNT = BitMasks.Bit4 | BitMasks.Bit5 | BitMasks.Bit6 | BitMasks.Bit7;
        internal const BitMasks ARC_CNT = BitMasks.Bit0 | BitMasks.Bit1 | BitMasks.Bit2 | BitMasks.Bit3;

        // Received Power Detector Register
        internal const BitMasks RPD = BitMasks.Bit0;

        // Receive Payload Width Pipe 0 Register
        internal const BitMasks RX_PW_P0 = BitMasks.Bit0 | BitMasks.Bit1 | BitMasks.Bit2 | BitMasks.Bit3 | BitMasks.Bit4 | BitMasks.Bit5;

        // Receive Payload Width Pipe 1 Register
        internal const BitMasks RX_PW_P1 = BitMasks.Bit0 | BitMasks.Bit1 | BitMasks.Bit2 | BitMasks.Bit3 | BitMasks.Bit4 | BitMasks.Bit5;

        // Receive Payload Width Pipe 2 Register
        internal const BitMasks RX_PW_P2 = BitMasks.Bit0 | BitMasks.Bit1 | BitMasks.Bit2 | BitMasks.Bit3 | BitMasks.Bit4 | BitMasks.Bit5;

        // Receive Payload Width Pipe 3 Register
        internal const BitMasks RX_PW_P3 = BitMasks.Bit0 | BitMasks.Bit1 | BitMasks.Bit2 | BitMasks.Bit3 | BitMasks.Bit4 | BitMasks.Bit5;

        // Receive Payload Width Pipe 4 Register
        internal const BitMasks RX_PW_P4 = BitMasks.Bit0 | BitMasks.Bit1 | BitMasks.Bit2 | BitMasks.Bit3 | BitMasks.Bit4 | BitMasks.Bit5;

        // Receive Payload Width Pipe 5 Register
        internal const BitMasks RX_PW_P5 = BitMasks.Bit0 | BitMasks.Bit1 | BitMasks.Bit2 | BitMasks.Bit3 | BitMasks.Bit4 | BitMasks.Bit5;

        // RF Channel Register
        internal const BitMasks RF_CH = BitMasks.Bit0 | BitMasks.Bit1 | BitMasks.Bit2 | BitMasks.Bit3 | BitMasks.Bit4 | BitMasks.Bit5 | BitMasks.Bit6;

        // RF Setup register
        internal const BitMasks CONT_WAVE = BitMasks.Bit6;
        internal const BitMasks RF_DR_LOW = BitMasks.Bit5;
        internal const BitMasks PLL_LOCK = BitMasks.Bit4;
        internal const BitMasks RF_DR_HIGH = BitMasks.Bit3;
        internal const BitMasks RF_PWR_HIGH = BitMasks.Bit2;
        internal const BitMasks RF_PWR_LOW = BitMasks.Bit1;

        // Setup Address Width Register
        internal const BitMasks AW = BitMasks.Bit0 | BitMasks.Bit1;

        // Setup Retransmission Register
        internal const BitMasks ARD = BitMasks.Bit4 | BitMasks.Bit5 | BitMasks.Bit6 | BitMasks.Bit7;
        internal const BitMasks ARC = BitMasks.Bit0 | BitMasks.Bit1 | BitMasks.Bit2 | BitMasks.Bit3;

        // Status Register
        internal const BitMasks RX_DR = BitMasks.Bit6;
        internal const BitMasks TX_DS = BitMasks.Bit5;
        internal const BitMasks MAX_RT = BitMasks.Bit4;
        internal const BitMasks RX_P_NO = BitMasks.Bit1 | BitMasks.Bit2 | BitMasks.Bit3;
        internal const BitMasks TX_FULL = BitMasks.Bit0;
    }
}