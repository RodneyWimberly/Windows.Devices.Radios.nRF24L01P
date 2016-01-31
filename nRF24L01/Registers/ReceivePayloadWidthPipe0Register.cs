namespace Windows.Devices.Radios.nRF24L01P.Registers
{
    /// <summary>
    ///   Number of bytes in RX payload in data pipe 0
    /// </summary>
    public class ReceivePayloadWidthPipe0Register : RegisterBase
    {
        public ReceivePayloadWidthPipe0Register(Radio radio) : base(radio, 1, Addresses.RX_PW_P0)
        {

        }

        public byte RX_PW_P0
        {
            get { return GetByteValue(5, Properties.RX_PW_P0); }
            set { SetByteValue(value, 5, Properties.RX_PW_P0); }
        }
    }
}
