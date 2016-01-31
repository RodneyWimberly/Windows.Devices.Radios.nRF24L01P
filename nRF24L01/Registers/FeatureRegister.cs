namespace Windows.Devices.Radios.nRF24L01.Registers
{
    /// <summary>
    ///   Feature Register
    /// </summary>
    public class FeatureRegister : RegisterBase
    {
        public FeatureRegister(Radio radio) : base(radio, 1, Addresses.FEATURE)
        {

        }

        public bool EN_DPL
        {
            get { return GetBitValue(Properties.EN_DPL); }
            set { FirstByte = SetBitValue(Properties.EN_DPL, value); }
        }

        public bool EN_ACK_PAY
        {
            get { return GetBitValue(Properties.EN_ACK_PAY); }
            set { FirstByte = SetBitValue(Properties.EN_ACK_PAY, value); }
        }

        public bool EN_DYN_ACK
        {
            get { return GetBitValue(Properties.EN_DYN_ACK); }
            set { FirstByte = SetBitValue(Properties.EN_DYN_ACK, value); }
        }

    }
}
