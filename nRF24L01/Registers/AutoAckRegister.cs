namespace Windows.Devices.Radios.nRF24L01.Registers
{
    public class AutoAckRegister : RegisterBase
    {
        public AutoAckRegister(Radio radio) : base(radio, 1, Constants.EN_AA)
        {

        }

        public bool EN_AA
        {
            get { return FirstByte != 0x0; }
            set { FirstByte = (byte)(value ? 0x3F : 0x0); }
        }

        public bool ENAA_P5
        {
            get { return GetBitValue(Constants.ENAA_P5); }
            set { FirstByte = SetBitValue(Constants.ENAA_P5, value); }
        }

        public bool ENAA_P4
        {
            get { return GetBitValue(Constants.ENAA_P4); }
            set { FirstByte = SetBitValue(Constants.ENAA_P4, value); }
        }

        public bool ENAA_P3
        {
            get { return GetBitValue(Constants.ENAA_P3); }
            set { FirstByte = SetBitValue(Constants.ENAA_P3, value); }
        }

        public bool ENAA_P2
        {
            get { return GetBitValue(Constants.ENAA_P2); }
            set { FirstByte = SetBitValue(Constants.ENAA_P2, value); }
        }

        public bool ENAA_P1
        {
            get { return GetBitValue(Constants.ENAA_P1); }
            set { FirstByte = SetBitValue(Constants.ENAA_P1, value); }
        }

        public bool ENAA_P0
        {
            get { return GetBitValue(Constants.ENAA_P0); }
            set { FirstByte = SetBitValue(Constants.ENAA_P0, value); }
        }
    }
}
