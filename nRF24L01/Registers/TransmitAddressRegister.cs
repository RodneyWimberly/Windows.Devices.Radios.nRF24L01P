namespace Windows.Devices.Radios.nRF24L01.Registers
{
    public class TransmitAddressRegister : RegisterBase
    {
        public TransmitAddressRegister(Radio radio) : base(radio, 5, Constants.TX_ADDR)
        {
        }

        public byte[] TX_ADDR
        {
            get { return Value; }
            set { Value = value; }
        }
    }
}
