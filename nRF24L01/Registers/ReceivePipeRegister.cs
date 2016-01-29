namespace Windows.Devices.Radios.nRF24L01.Registers
{
    public class ReceivePipeRegister : RegisterBase
    {
        public ReceivePipeRegister(Radio radio) : base(radio, 1, Constants.EN_RXADDR)
        {

        }

        public bool ERX_P5
        {
            get { return GetBitValue(Constants.ERX_P5); }
            set { FirstByte = SetBitValue(Constants.ERX_P5, value); }
        }

        public bool ERX_P4
        {
            get { return GetBitValue(Constants.ERX_P4); }
            set { FirstByte = SetBitValue(Constants.ERX_P4, value); }
        }

        public bool ERX_P3
        {
            get { return GetBitValue(Constants.ERX_P3); }
            set { FirstByte = SetBitValue(Constants.ERX_P3, value); }
        }

        public bool ERX_P2
        {
            get { return GetBitValue(Constants.ERX_P2); }
            set { FirstByte = SetBitValue(Constants.ERX_P2, value); }
        }

        public bool ERX_P1
        {
            get { return GetBitValue(Constants.ERX_P1); }
            set { FirstByte = SetBitValue(Constants.ERX_P1, value); }
        }

        public bool ERX_P0
        {
            get { return GetBitValue(Constants.ERX_P0); }
            set { FirstByte = SetBitValue(Constants.ERX_P0, value); }
        }
    }
}
