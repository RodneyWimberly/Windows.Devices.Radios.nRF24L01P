using Windows.Devices.Radios.nRF24L01P.Interfaces;

namespace Windows.Devices.Radios.nRF24L01P.Registers
{
    /// <summary>
    ///   FIFO Status Register
    /// </summary>
    public class FifoStatusRegister : RegisterBase
    {
        public FifoStatusRegister(ICommandProcessor commandProcessor) :
            base(commandProcessor, 1, RegisterAddresses.FIFO_STATUS)
        { }

        /// <summary>
        /// Used for a P TX device 
        /// Pulse the rfce high for at least lops to Reuse last
        /// transmitted payload.TX payload reuse is active
        /// until W TX PAYLOAD or FLUSH TX is executed.
        /// TX REUSE is set by the SPI command
        /// REUSE TX PL, and is reset by the SPI commands
        /// W TX PAYLOAD or FLUSH TX
        /// </summary>
        public bool TransmitPayloadReuse
        {
            get { return GetBoolProperty(PropertyMasks.TX_REUSE); }
            set { SetBoolProperty(PropertyMasks.TX_REUSE, value); }
        }

        /// <summary>
        /// TX FIFO full flag. 1: TX FIFO full. O: Available loca 
        /// tions in TX FIFO.
        /// </summary>
        public bool TransmitFifoFull
        {
            get { return GetBoolProperty(PropertyMasks.TX_FULL); }
            set { SetBoolProperty(PropertyMasks.TX_FULL, value); }
        }

        /// <summary>
        /// TX FIFO empty flag. 
        /// 1: TX FIFO empty.
        /// O: Data in TX FIFO.     
        /// </summary>
        public bool TransmitFifoEmpty
        {
            get { return GetBoolProperty(PropertyMasks.TX_EMPTY); }
            set { SetBoolProperty(PropertyMasks.TX_EMPTY, value); }
        }

        /// <summary>
        /// RX FIFO full flag. 
        /// 1: RX FIFO full.
        /// 0: Available locations in RX FIFO.
        /// </summary>
        public bool ReceiveFifoFull
        {
            get { return GetBoolProperty(PropertyMasks.RX_FULL); }
            set { SetBoolProperty(PropertyMasks.RX_FULL, value); }
        }

        /// <summary>
        /// RX FIFO empty flag. 
        /// 1: RX FIFO empty.
        /// O: Data in RX FIFO.
        /// </summary>
        public bool ReveiveFifoEmpty
        {
            get { return GetBoolProperty(PropertyMasks.RX_EMPTY); }
            set { SetBoolProperty(PropertyMasks.RX_EMPTY, value); }
        }
    }
}
