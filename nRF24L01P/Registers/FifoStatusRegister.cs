using Windows.Devices.Radios.nRF24L01P.Interfaces;

namespace Windows.Devices.Radios.nRF24L01P.Registers
{
    /// <summary>
    ///   FIFO Status Register
    /// </summary>
    public class FifoStatusRegister : RegisterBase
    {
        public FifoStatusRegister(ICommandProcessor commandProcessor) :
            base(commandProcessor, 1, RegisterAddresses.FIFO_STATUS, RegisterDefaults.FIFO_STATUS, "FIFO_STATUS")
        { }

        /// <summary>
        /// Used for a PTX device 
        /// Pulse the rfce high for at least 10us to Reuse last transmitted payload.
        /// </summary>
        /// <remarks> 
        /// TX payload reuse is active
        /// until W_TX_PAYLOAD or FLUSH_TX is executed.
        /// TX_REUSE is set by the SPI command
        /// REUSE_TX_PL, and is reset by the SPI commands
        /// W_TX_PAYLOAD or FLUSH_TX
        /// </remarks>
        public bool TransmitPayloadReuse => GetBoolProperty(PropertyMasks.TX_REUSE);

        /// <summary>
        /// TX FIFO full flag.
        /// </summary>
        /// <returns>
        /// 1: TX FIFO full. 
        /// O: Available locations in TX FIFO.
        /// </returns>
        public bool TransmitFifoFull => GetBoolProperty(PropertyMasks.TX_FULL);

        /// <summary>
        /// TX FIFO empty flag. 
        /// </summary>
        /// <returns>
        /// 1: TX FIFO empty.
        /// O: Data in TX FIFO.     
        /// </returns>
        public bool TransmitFifoEmpty => GetBoolProperty(PropertyMasks.TX_EMPTY);

        /// <summary>
        /// RX FIFO full flag. 
        /// </summary>
        /// <returns>
        /// 1: RX FIFO full.
        /// 0: Available locations in RX FIFO.
        /// </returns>
        public bool ReceiveFifoFull => GetBoolProperty(PropertyMasks.RX_FULL);

        /// <summary>
        /// RX FIFO empty flag. 
        /// </summary>
        /// <returns>
        /// 1: RX FIFO empty.
        /// O: Data in RX FIFO.
        /// </returns>
        public bool ReveiveFifoEmpty => GetBoolProperty(PropertyMasks.RX_EMPTY);
    }
}
