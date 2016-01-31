namespace Windows.Devices.Radios.nRF24L01.Registers
{
    /// <summary>
    ///   Enable dynamic payload length
    /// </summary>
    public class DynamicPayloadLengthRegister : RegisterBase
    {
        public DynamicPayloadLengthRegister(Radio radio) : base(radio, 1, Addresses.DYNPD)
        {

        }

        public bool DPL_P5
        {
            get { return GetBitValue(Properties.DPL_P5); }
            set { FirstByte = SetBitValue(Properties.DPL_P5, value); }
        }

        public bool DPL_P4
        {
            get { return GetBitValue(Properties.DPL_P4); }
            set { FirstByte = SetBitValue(Properties.DPL_P4, value); }
        }

        public bool DPL_P3
        {
            get { return GetBitValue(Properties.DPL_P3); }
            set { FirstByte = SetBitValue(Properties.DPL_P3, value); }
        }

        public bool DPL_P2
        {
            get { return GetBitValue(Properties.DPL_P2); }
            set { FirstByte = SetBitValue(Properties.DPL_P2, value); }
        }

        public bool DPL_P1
        {
            get { return GetBitValue(Properties.DPL_P1); }
            set { FirstByte = SetBitValue(Properties.DPL_P1, value); }
        }
        public bool DPL_P0
        {
            get { return GetBitValue(Properties.DPL_P0); }
            set { FirstByte = SetBitValue(Properties.DPL_P0, value); }
        }
    }
}
