namespace Windows.Devices.Radios.nRF24L01.Registers
{
    public class ReceivePipe1PayloadWidthRegister : RegisterBase
    {
        public ReceivePipe1PayloadWidthRegister(Radio radio) : base(radio, 1, Constants.RX_PW_P1)
        {

        }

        public byte RX_PW_P1
        {
            get { return GetByteValue(5, 0); }
            set { FirstByte = SetByteValue(value, 5, 0); }
        }
    }
}
