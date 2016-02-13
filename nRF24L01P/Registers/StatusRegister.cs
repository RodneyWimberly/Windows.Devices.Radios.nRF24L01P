using Windows.Devices.Radios.nRF24L01P.Interfaces;

namespace Windows.Devices.Radios.nRF24L01P.Registers
{
    /// <summary>
    ///   Status Register (In parallel to the SPI command word applied on the MOSI pin, the STATUS register is shifted serially out on the MISO pin)
    /// </summary>
    public class StatusRegister : RegisterBase
    {
        public StatusRegister(ICommandProcessor commandProcessor) : base(commandProcessor, 1, RegisterAddresses.STATUS)
        {
        }

        /// <summary>
        /// Data Ready RX FIFO interrupt. Asserted when 
        /// new data arrives RX FIFOC.
        /// Write 1 to clear bit.
        /// </summary>
        public bool ReceiveDataReady
        {
            get { return GetBoolProperty(PropertyMasks.RX_DR); }
            set { SetBoolProperty(PropertyMasks.RX_DR, value); }
        }

        /// <summary>
        /// Data Sent TX FIFO interrupt. Asserted when 
        /// packet transmitted on TX.If AUTO ACK is acti- 
        /// vated, this bit is set high only when ACK is 
        /// received.
        /// Write 1 to clear bit.
        /// </summary>
        public bool TransmitDataSent
        {
            get { return GetBoolProperty(PropertyMasks.TX_DS); }
            set { SetBoolProperty(PropertyMasks.TX_DS, value); }
        }

        /// <summary>
        /// Maximum number of TX retransmits interrupt 
        /// Write 1 to clear bit.If MP„x RT is asserted it must
        /// be cleared to enable further communication.
        /// </summary>
        public bool MaximunTransmitRetries
        {
            get { return GetBoolProperty(PropertyMasks.MAX_RT); }
            set { SetBoolProperty(PropertyMasks.MAX_RT, value); }
        }

        /// <summary>
        /// Data pipe number for the payload available for 
        /// reading from RX FIFO 
        /// 000-101: Data Pipe Number 
        /// 110: Not Used 
        /// 111: RX FIFO Empty
        /// </summary>
        public byte ReceiveDataPipeNumber => GetByteProperty(PropertyMasks.RX_P_NO);

        /// <summary>
        /// TX FIFO full flag. 
        /// 1: TX FIFO full.
        /// 0: Available locations in TX FIFO.
        /// </summary>
        public bool TransmitFifoFull => GetBoolProperty(PropertyMasks.TX_FULL);
    }
}
