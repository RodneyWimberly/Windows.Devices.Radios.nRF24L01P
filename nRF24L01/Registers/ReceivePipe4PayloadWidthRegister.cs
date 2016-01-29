namespace Windows.Devices.Radios.nRF24L01.Registers
{
    public class ReceivePipe4PayloadWidthRegister : RegisterBase
    {
        public ReceivePipe4PayloadWidthRegister(Radio radio) : base(radio, 1, Constants.RX_PW_P4)
        {

        }

        public byte RX_PW_P4
        {
            get { return GetByteValue(5, 0); }
            set { FirstByte = SetByteValue(value, 5, 0); }
        }
    }
}
