namespace Windows.Devices.Radios.nRF24L01P.Registers
{
    /// <summary>
    ///   FIFO Status Register
    /// </summary>
    public class FifoStatusRegister : RegisterBase
    {
        public FifoStatusRegister(Radio radio) : base(radio, 1, Addresses.FIFO_STATUS)
        {

        }

        public bool TX_REUSE
        {
            get { return GetBitValue(Properties.TX_REUSE); }
            set { SetBitValue(Properties.TX_REUSE, value); }
        }

        public bool TX_FULL
        {
            get { return GetBitValue(Properties.TX_FULL); }
            set { SetBitValue(Properties.TX_FULL, value); }
        }

        public bool TX_EMPTY
        {
            get { return GetBitValue(Properties.TX_EMPTY); }
            set { SetBitValue(Properties.TX_EMPTY, value); }
        }

        public bool RX_FULL
        {
            get { return GetBitValue(Properties.RX_FULL); }
            set { SetBitValue(Properties.RX_FULL, value); }
        }

        public bool RX_EMPTY
        {
            get { return GetBitValue(Properties.RX_EMPTY); }
            set { SetBitValue(Properties.RX_EMPTY, value); }
        }
    }
}
