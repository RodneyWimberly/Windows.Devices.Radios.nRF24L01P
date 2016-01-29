namespace Windows.Devices.Radios.nRF24L01.Registers
{
    public class DynamicPayloadRegister : RegisterBase
    {
        public DynamicPayloadRegister(Radio radio) : base(radio, 1, Constants.DYNPD)
        {

        }

        public bool DPL_P5
        {
            get { return GetBitValue(Constants.DPL_P5); }
            set { FirstByte = SetBitValue(Constants.DPL_P5, value); }
        }

        public bool DPL_P4
        {
            get { return GetBitValue(Constants.DPL_P4); }
            set { FirstByte = SetBitValue(Constants.DPL_P4, value); }
        }

        public bool DPL_P3
        {
            get { return GetBitValue(Constants.DPL_P3); }
            set { FirstByte = SetBitValue(Constants.DPL_P3, value); }
        }

        public bool DPL_P2
        {
            get { return GetBitValue(Constants.DPL_P2); }
            set { FirstByte = SetBitValue(Constants.DPL_P2, value); }
        }

        public bool DPL_P1
        {
            get { return GetBitValue(Constants.DPL_P1); }
            set { FirstByte = SetBitValue(Constants.DPL_P1, value); }
        }
        public bool DPL_P0
        {
            get { return GetBitValue(Constants.DPL_P0); }
            set { FirstByte = SetBitValue(Constants.DPL_P0, value); }
        }
    }
}
