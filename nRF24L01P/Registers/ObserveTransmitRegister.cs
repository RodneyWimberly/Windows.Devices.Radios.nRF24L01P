using Windows.Devices.Radios.nRF24L01P.Interfaces;

namespace Windows.Devices.Radios.nRF24L01P.Registers
{
    /// <summary>
    ///   Observe Transmit register
    /// </summary>
    public class ObserveTransmitRegister : RegisterBase
    {
        public ObserveTransmitRegister(ICommandProcessor commandProcessor) :
            base(commandProcessor, 1, RegisterAddresses.OBSERVE_TX)
        { }

        /// <summary>
        /// Count lost packets. The counter is overflow pro- 
        /// tected to 15, and discontinues at max until reset.
        /// The counter is reset by writing to RF CH
        /// </summary>
        public byte PacketLossCount
        {
            get { return GetByteProperty(PropertyMasks.PLOS_CNT); }
            set { SetByteProperty(PropertyMasks.PLOS_CNT, value); }
        }

        /// <summary>
        /// Count retransmitted packets. The counter is reset 
        /// when transmission of a new packet starts.
        /// </summary>
        public byte RetransmitedPacketsCount
        {
            get { return GetByteProperty(PropertyMasks.ARC_CNT); }
            set { SetByteProperty(PropertyMasks.ARC_CNT, value); }
        }
    }
}
