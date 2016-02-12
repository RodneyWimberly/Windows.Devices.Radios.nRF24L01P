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

        public bool TX_REUSE
        {
            get { return GetBoolProperty(PropertyMasks.TX_REUSE); }
            set { SetBoolProperty(PropertyMasks.TX_REUSE, value); }
        }

        public bool TX_FULL
        {
            get { return GetBoolProperty(PropertyMasks.TX_FULL); }
            set { SetBoolProperty(PropertyMasks.TX_FULL, value); }
        }

        public bool TX_EMPTY
        {
            get { return GetBoolProperty(PropertyMasks.TX_EMPTY); }
            set { SetBoolProperty(PropertyMasks.TX_EMPTY, value); }
        }

        public bool RX_FULL
        {
            get { return GetBoolProperty(PropertyMasks.RX_FULL); }
            set { SetBoolProperty(PropertyMasks.RX_FULL, value); }
        }

        public bool RX_EMPTY
        {
            get { return GetBoolProperty(PropertyMasks.RX_EMPTY); }
            set { SetBoolProperty(PropertyMasks.RX_EMPTY, value); }
        }
    }
}
