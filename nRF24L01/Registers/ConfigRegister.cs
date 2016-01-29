namespace Windows.Devices.Radios.nRF24L01.Registers
{
    public class ConfigRegister : RegisterBase
    {
        public ConfigRegister(Radio radio) : base(radio, 1, Constants.CONFIG)
        {

        }

        public bool MASK_RX_DR
        {
            get { return GetBitValue(Constants.MASK_RX_DR); }
            set { FirstByte = SetBitValue(Constants.MASK_RX_DR, value); }
        }
        public bool MASK_TX_DS
        {
            get { return GetBitValue(Constants.MASK_TX_DS); }
            set { FirstByte = SetBitValue(Constants.MASK_TX_DS, value); }
        }
        public bool MASK_MAX_RT
        {
            get { return GetBitValue(Constants.MASK_MAX_RT); }
            set { FirstByte = SetBitValue(Constants.MASK_MAX_RT, value); }
        }
        public bool EN_CRC
        {
            get { return GetBitValue(Constants.EN_CRC); }
            set { FirstByte = SetBitValue(Constants.EN_CRC, value); }
        }
        public bool CRCO
        {
            get { return GetBitValue(Constants.CRCO); }
            set { FirstByte = SetBitValue(Constants.CRCO, value); }
        }
        public bool PWR_UP
        {
            get { return GetBitValue(Constants.PWR_UP); }
            set { FirstByte = SetBitValue(Constants.PWR_UP, value); }
        }
        public bool PRIM_RX
        {
            get { return GetBitValue(Constants.PRIM_RX); }
            set { FirstByte = SetBitValue(Constants.PRIM_RX, value); }
        }
    }
}
