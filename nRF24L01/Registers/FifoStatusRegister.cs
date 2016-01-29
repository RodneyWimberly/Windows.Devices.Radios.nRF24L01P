namespace Windows.Devices.Radios.nRF24L01.Registers
{
    public class FifoStatusRegister : RegisterBase
    {
        public FifoStatusRegister(Radio radio) : base(radio, 1, Constants.FIFO_STATUS)
        {

        }

        public bool TX_REUSE
        {
            get { return GetBitValue(Constants.TX_REUSE); }
            set { FirstByte = SetBitValue(Constants.TX_REUSE, value); }
        }

        public bool TX_FULL
        {
            get { return GetBitValue(Constants.TX_FULL); }
            set { FirstByte = SetBitValue(Constants.TX_FULL, value); }
        }

        public bool TX_EMPTY
        {
            get { return GetBitValue(Constants.TX_EMPTY); }
            set { FirstByte = SetBitValue(Constants.TX_EMPTY, value); }
        }

        public bool RX_FULL
        {
            get { return GetBitValue(Constants.RX_FULL); }
            set { FirstByte = SetBitValue(Constants.RX_FULL, value); }
        }

        public bool RX_EMPTY
        {
            get { return GetBitValue(Constants.RX_EMPTY); }
            set { FirstByte = SetBitValue(Constants.RX_EMPTY, value); }
        }
    }
}
