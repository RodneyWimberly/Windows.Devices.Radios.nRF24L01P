namespace Windows.Devices.Radios.nRF24L01.Registers
{
    public class ReceivePipe5PayloadWidthRegister : RegisterBase
    {
        public ReceivePipe5PayloadWidthRegister(Radio radio) : base(radio, 1, Constants.RX_PW_P5)
        {

        }

        public byte RX_PW_P5
        {
            get { return GetByteValue(5, 0); }
            set { FirstByte = SetByteValue(value, 5, 0); }
        }
    }
}
