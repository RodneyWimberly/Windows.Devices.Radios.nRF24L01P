namespace Windows.Devices.Radios.nRF24L01.Registers
{
    public class ChannelRegister : RegisterBase
    {
        public ChannelRegister(Radio radio) : base(radio, 1, Constants.RF_CH)
        {

        }

        public byte RF_CH
        {
            get { return GetByteValue(6, 0); }
            set { FirstByte = SetByteValue(value, 6, 0); }
        }
    }
}
