namespace Windows.Devices.Radios.nRF24L01.Registers
{
    public class ReceivePipe0PayloadWidthRegister : RegisterBase
    {
        public ReceivePipe0PayloadWidthRegister(Radio radio) : base(radio, 1, Constants.RX_PW_P0)
        {

        }

        public byte RX_PW_P0
        {
            get { return GetByteValue(5, 0); }
            set { FirstByte = SetByteValue(value, 5, 0); }
        }
    }
}
