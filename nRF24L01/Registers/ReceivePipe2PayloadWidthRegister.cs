namespace Windows.Devices.Radios.nRF24L01.Registers
{
    public class ReceivePipe2PayloadWidthRegister : RegisterBase
    {
        public ReceivePipe2PayloadWidthRegister(Radio radio) : base(radio, 1, Constants.RX_PW_P2)
        {

        }

        public byte RX_PW_P2
        {
            get { return GetByteValue(5, 0); }
            set { FirstByte = SetByteValue(value, 5, 0); }
        }
    }
}
