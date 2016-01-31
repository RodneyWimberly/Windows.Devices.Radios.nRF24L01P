namespace Windows.Devices.Radios.nRF24L01.Registers
{
    /// <summary>
    ///   Enabled RX Addresses
    /// </summary>
    public class EnableReceiveAddressRegister : RegisterBase
    {
        public EnableReceiveAddressRegister(Radio radio) : base(radio, 1, Addresses.EN_RXADDR)
        {

        }

        public bool ERX_P5
        {
            get { return GetBitValue(Properties.ERX_P5); }
            set { SetBitValue(Properties.ERX_P5, value); }
        }

        public bool ERX_P4
        {
            get { return GetBitValue(Properties.ERX_P4); }
            set { SetBitValue(Properties.ERX_P4, value); }
        }

        public bool ERX_P3
        {
            get { return GetBitValue(Properties.ERX_P3); }
            set { SetBitValue(Properties.ERX_P3, value); }
        }

        public bool ERX_P2
        {
            get { return GetBitValue(Properties.ERX_P2); }
            set { SetBitValue(Properties.ERX_P2, value); }
        }

        public bool ERX_P1
        {
            get { return GetBitValue(Properties.ERX_P1); }
            set { SetBitValue(Properties.ERX_P1, value); }
        }

        public bool ERX_P0
        {
            get { return GetBitValue(Properties.ERX_P0); }
            set { SetBitValue(Properties.ERX_P0, value); }
        }
    }
}
