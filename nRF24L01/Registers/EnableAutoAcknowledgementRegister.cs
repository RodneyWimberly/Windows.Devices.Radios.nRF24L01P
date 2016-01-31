namespace Windows.Devices.Radios.nRF24L01.Registers
{
    /// <summary>
    ///   Enable 'Auto Acknowledgment' Function. Disable this functionality to be compatible with nRF2401.
    /// </summary>
    public class EnableAutoAcknowledgementRegister : RegisterBase
    {
        public EnableAutoAcknowledgementRegister(Radio radio) : base(radio, 1, Addresses.EN_AA)
        {

        }

        public bool EN_AA
        {
            get { return FirstByte != 0x0; }
            set { FirstByte = (byte)(value ? 0x3F : 0x0); }
        }

        public bool ENAA_P5
        {
            get { return GetBitValue(Properties.ENAA_P5); }
            set { FirstByte = SetBitValue(Properties.ENAA_P5, value); }
        }

        public bool ENAA_P4
        {
            get { return GetBitValue(Properties.ENAA_P4); }
            set { FirstByte = SetBitValue(Properties.ENAA_P4, value); }
        }

        public bool ENAA_P3
        {
            get { return GetBitValue(Properties.ENAA_P3); }
            set { FirstByte = SetBitValue(Properties.ENAA_P3, value); }
        }

        public bool ENAA_P2
        {
            get { return GetBitValue(Properties.ENAA_P2); }
            set { FirstByte = SetBitValue(Properties.ENAA_P2, value); }
        }

        public bool ENAA_P1
        {
            get { return GetBitValue(Properties.ENAA_P1); }
            set { FirstByte = SetBitValue(Properties.ENAA_P1, value); }
        }

        public bool ENAA_P0
        {
            get { return GetBitValue(Properties.ENAA_P0); }
            set { FirstByte = SetBitValue(Properties.ENAA_P0, value); }
        }
    }
}
