namespace Windows.Devices.Radios.nRF24L01.Registers
{
    public class ReceivePipe1AddressRegister : RegisterBase
    {
        public ReceivePipe1AddressRegister(Radio radio) : base(radio, 5, Constants.RX_ADDR_P1)
        {

        }

        public byte[] RX_ADDR_P1
        {
            get { return Value; }
            set { Value = value; }
        }
    }
}
