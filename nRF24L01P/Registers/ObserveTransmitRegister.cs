namespace Windows.Devices.Radios.nRF24L01P.Registers
{
    /// <summary>
    ///   Observe Transmit register
    /// </summary>
    public class ObserveTransmitRegister : RegisterBase
    {
        public ObserveTransmitRegister(ICommandProcessor commandProcessor) : base(commandProcessor, 1, RegisterAddresses.OBSERVE_TX)
        {

        }

        public byte PLOS_CNT
        {
            get { return GetByteValue(7, Properties.PLOS_CNT); }
            set { SetByteValue(value, 7, Properties.PLOS_CNT); }
        }

        public byte ARC_CNT
        {
            get { return GetByteValue(3, Properties.ARC_CNT); }
            set { SetByteValue(value, 3, Properties.ARC_CNT); }
        }
    }
}
