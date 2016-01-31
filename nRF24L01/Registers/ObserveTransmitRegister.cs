namespace Windows.Devices.Radios.nRF24L01.Registers
{
    /// <summary>
    ///   Observe Transmit register
    /// </summary>
    public class ObserveTransmitRegister : RegisterBase
    {
        public ObserveTransmitRegister(Radio radio) : base(radio, 1, Addresses.OBSERVE_TX)
        {

        }

        public byte PLOS_CNT
        {
            get { return GetByteValue(7, Properties.PLOS_CNT); }
            set { FirstByte = SetByteValue(value, 7, Properties.PLOS_CNT); }
        }

        public byte ARC_CNT
        {
            get { return GetByteValue(3, Properties.ARC_CNT); }
            set { FirstByte = SetByteValue(value, 3, Properties.ARC_CNT); }
        }
    }
}
