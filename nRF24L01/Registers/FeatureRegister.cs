namespace Windows.Devices.Radios.nRF24L01.Registers
{
    public class FeatureRegister : RegisterBase
    {
        public FeatureRegister(Radio radio) : base(radio, 1, Constants.FEATURE)
        {

        }

        public bool EN_DPL
        {
            get { return GetBitValue(Constants.EN_DPL); }
            set { FirstByte = SetBitValue(Constants.EN_DPL, value); }
        }

        public bool EN_ACK_PAY
        {
            get { return GetBitValue(Constants.EN_ACK_PAY); }
            set { FirstByte = SetBitValue(Constants.EN_ACK_PAY, value); }
        }

        public bool EN_DYN_ACK
        {
            get { return GetBitValue(Constants.EN_DYN_ACK); }
            set { FirstByte = SetBitValue(Constants.EN_DYN_ACK, value); }
        }

    }
}
