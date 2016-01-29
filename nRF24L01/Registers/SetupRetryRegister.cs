namespace Windows.Devices.Radios.nRF24L01.Registers
{
    public class SetupRetryRegister : RegisterBase
    {
        public SetupRetryRegister(Radio radio) : base(radio, 1, Constants.SETUP_RETR)
        {

        }

        public byte ARD
        {
            get { return GetByteValue(7, 4); }
            set { FirstByte = SetByteValue(value, 7, 4); }
        }

        public byte ARC
        {
            get { return GetByteValue(3, 0); }
            set { FirstByte = SetByteValue(value, 3, 0); }
        }
    }
}
