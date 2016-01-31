namespace Windows.Devices.Radios.nRF24L01.Registers
{
    /// <summary>
    ///   Configuration Register
    /// </summary>
    public class ConfigurationRegister : RegisterBase
    {
        public ConfigurationRegister(Radio radio) : base(radio, 1, Addresses.CONFIG)
        {

        }

        public bool MASK_RX_DR
        {
            get { return GetBitValue(Properties.MASK_RX_DR); }
            set { FirstByte = SetBitValue(Properties.MASK_RX_DR, value); }
        }
        public bool MASK_TX_DS
        {
            get { return GetBitValue(Properties.MASK_TX_DS); }
            set { FirstByte = SetBitValue(Properties.MASK_TX_DS, value); }
        }
        public bool MASK_MAX_RT
        {
            get { return GetBitValue(Properties.MASK_MAX_RT); }
            set { FirstByte = SetBitValue(Properties.MASK_MAX_RT, value); }
        }
        public bool EN_CRC
        {
            get { return GetBitValue(Properties.EN_CRC); }
            set { FirstByte = SetBitValue(Properties.EN_CRC, value); }
        }
        public bool CRCO
        {
            get { return GetBitValue(Properties.CRCO); }
            set { FirstByte = SetBitValue(Properties.CRCO, value); }
        }
        public bool PWR_UP
        {
            get { return GetBitValue(Properties.PWR_UP); }
            set { FirstByte = SetBitValue(Properties.PWR_UP, value); }
        }
        public bool PRIM_RX
        {
            get { return GetBitValue(Properties.PRIM_RX); }
            set { FirstByte = SetBitValue(Properties.PRIM_RX, value); }
        }
    }
}
