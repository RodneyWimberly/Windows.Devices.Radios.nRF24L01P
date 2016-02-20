using Windows.Devices.Radios.nRF24L01P.Interfaces;

namespace Windows.Devices.Radios.nRF24L01P.Registers
{
    /// <summary>
    ///   Status Register 
    /// </summary>
    /// <remarks>
    /// (In parallel to the SPI command word applied on the MOSI pin,
    /// the STATUS register is shifted serially out on the MISO pin)
    /// </remarks>
    public class StatusRegister : RegisterBase
    {
        public StatusRegister(ICommandProcessor commandProcessor) :
            base(commandProcessor, 1, RegisterAddresses.STATUS)
        { }

        /// <summary>
        /// Data Ready RX FIFO interrupt. Asserted when 
        /// new data arrives RX FIFOC.
        /// </summary>
        /// <remarks>
        /// Write 1 to clear bit.
        /// </remarks>
        public bool ReceiveDataReady
        {
            get { return GetBoolProperty(PropertyMasks.RX_DR); }
            set { SetBoolProperty(PropertyMasks.RX_DR, value); }
        }

        /// <summary>
        /// Data Sent TX FIFO interrupt. Asserted when 
        /// packet transmitted on TX.If AUTO ACK is activated,
        /// this bit is set high only when ACK is received.
        /// </summary>
        /// <remarks>
        /// Write 1 to clear bit.
        /// </remarks>
        public bool TransmitDataSent
        {
            get { return GetBoolProperty(PropertyMasks.TX_DS); }
            set { SetBoolProperty(PropertyMasks.TX_DS, value); }
        }

        /// <summary>
        /// Maximum number of TX retransmits interrupt 
        /// </summary>
        /// <remarks>
        /// Write 1 to clear bit.If MP„x RT is asserted it must
        /// be cleared to enable further communication.
        /// </remarks>
        public bool MaximunTransmitRetries
        {
            get { return GetBoolProperty(PropertyMasks.MAX_RT); }
            set { SetBoolProperty(PropertyMasks.MAX_RT, value); }
        }

        /// <summary>
        /// Data pipe number for the payload available for 
        /// reading from RX FIFO 
        /// </summary>
        /// <returns>
        /// 000-101: Data Pipe Number 
        /// 110: Not Used 
        /// 111: RX FIFO Empty
        /// </returns>
        public byte ReceiveDataPipeNumber => GetByteProperty(PropertyMasks.RX_P_NO);

        /// <summary>
        /// TX FIFO full flag. 
        /// </summary>
        /// <returns>
        /// 1: TX FIFO full.
        /// 0: Available locations in TX FIFO.
        /// </returns>
        public bool TransmitFifoFull => GetBoolProperty(PropertyMasks.TX_FULL);
    }
}
