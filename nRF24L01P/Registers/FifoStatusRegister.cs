using Windows.Devices.Radios.nRF24L01P.Interfaces;

namespace Windows.Devices.Radios.nRF24L01P.Registers
{
    /// <summary>
    ///   FIFO Status Register
    /// </summary>
    public class FifoStatusRegister : RegisterBase
    {
        public FifoStatusRegister(ICommandProcessor commandProcessor) : base(commandProcessor, 1, RegisterAddresses.FIFO_STATUS)
        {

        }

        public bool TransmitPayloadReuse
        {
            get { return GetBoolProperty(PropertyMasks.TX_REUSE); }
            set { SetBoolProperty(PropertyMasks.TX_REUSE, value); }
        }

        public bool TransmitFifoFull
        {
            get { return GetBoolProperty(PropertyMasks.TX_FULL); }
            set { SetBoolProperty(PropertyMasks.TX_FULL, value); }
        }

        public bool TransmitFifoEmpty
        {
            get { return GetBoolProperty(PropertyMasks.TX_EMPTY); }
            set { SetBoolProperty(PropertyMasks.TX_EMPTY, value); }
        }

        public bool ReceiveFifoFull
        {
            get { return GetBoolProperty(PropertyMasks.RX_FULL); }
            set { SetBoolProperty(PropertyMasks.RX_FULL, value); }
        }

        public bool ReveiveFifoEmpty
        {
            get { return GetBoolProperty(PropertyMasks.RX_EMPTY); }
            set { SetBoolProperty(PropertyMasks.RX_EMPTY, value); }
        }
    }
}
