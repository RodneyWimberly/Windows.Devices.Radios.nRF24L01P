namespace Windows.Devices.Radios.nRF24L01.Registers
{
    public class AddressWidthRegister : RegisterBase
    {
        public AddressWidthRegister(Radio radio) : base(radio, 1, Constants.SETUP_AW)
        {

        }

        public byte AW
        {
            get { return GetByteValue(1, 0); }
            set { FirstByte = SetByteValue(value, 1, 0); }
        }
    }
}
