namespace Windows.Devices.Radios.nRF24L01.Registers
{
    public class ReceivePipe3PayloadWidthRegister : RegisterBase
    {
        public ReceivePipe3PayloadWidthRegister(Radio radio) : base(radio, 1, Constants.RX_PW_P3)
        {

        }

        public byte RX_PW_P3
        {
            get { return GetByteValue(5, 0); }
            set { FirstByte = SetByteValue(value, 5, 0); }
        }
    }
}
