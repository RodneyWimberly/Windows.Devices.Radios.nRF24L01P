namespace Windows.Devices.Radios.nRF24L01.Registers
{
    /// <summary>
    ///   Setup of Address Widths (common for all data pipes)
    /// </summary>
    public class SetupAddressWidthRegister : RegisterBase
    {
        public SetupAddressWidthRegister(Radio radio) : base(radio, 1, Addresses.SETUP_AW)
        {

        }

        public byte AW
        {
            get { return GetByteValue(Properties.AW, 0); }
            set { FirstByte = SetByteValue(value, Properties.AW, 0); }
        }
    }
}
