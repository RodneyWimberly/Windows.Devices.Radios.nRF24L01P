using Windows.Devices.Radios.nRF24L01P.Interfaces;

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

        public byte CountPacketLoss
        {
            get { return GetByteProperty(PropertyMasks.PLOS_CNT); }
            set { SetByteProperty(PropertyMasks.PLOS_CNT, value ); }
        }

        public byte CountRetransmitedPackets
        {
            get { return GetByteProperty(PropertyMasks.ARC_CNT); }
            set { SetByteProperty(PropertyMasks.ARC_CNT, value); }
        }
    }
}
