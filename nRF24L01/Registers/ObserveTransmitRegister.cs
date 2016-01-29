namespace Windows.Devices.Radios.nRF24L01.Registers
{
    public class ObserveTransmitRegister : RegisterBase
    {
        public ObserveTransmitRegister(Radio radio) : base(radio, 1, Constants.OBSERVE_TX)
        {

        }

        public byte PLOS_CNT
        {
            get { return GetByteValue(7, 4); }
            set { FirstByte = SetByteValue(value, 7, 4); }
        }

        public byte ARC_CNT
        {
            get { return GetByteValue(3, 0); }
            set { FirstByte = SetByteValue(value, 3, 0); }
        }
    }
}
