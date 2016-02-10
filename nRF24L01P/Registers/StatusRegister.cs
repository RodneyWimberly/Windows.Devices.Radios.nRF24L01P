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
        /// Receive Data Ready6
        /// </summary>
        public bool RX_DR
        {
            get { return GetBitValue(Properties.RX_DR); }
            set { SetBitValue(Properties.RX_DR, value); }
        }

        public bool TX_DS
        {
            get { return GetBitValue(Properties.TX_DS); }
            set { SetBitValue(Properties.TX_DS, value); }
        }

        public bool MAX_RT
        {
            get { return GetBitValue(Properties.MAX_RT); }
            set { SetBitValue(Properties.MAX_RT, value); }
        }

        public byte RX_P_NO => GetByteValue(3, 1);

        public bool TX_FULL => GetBitValue(Properties.TX_FULL);
    }
}
