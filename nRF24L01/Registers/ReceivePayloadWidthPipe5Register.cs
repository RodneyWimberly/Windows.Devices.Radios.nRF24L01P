namespace Windows.Devices.Radios.nRF24L01P.Registers
{
    /// <summary>
    ///   Number of bytes in RX payload in data pipe 5
    /// </summary>
    public class ReceivePayloadWidthPipe5Register : RegisterBase
    {
        public ReceivePayloadWidthPipe5Register(Radio radio) : base(radio, 1, Addresses.RX_PW_P5)
        {

        }

        public byte RX_PW_P5
        {
            get { return GetByteValue(5, Properties.RX_PW_P5); }
            set { SetByteValue(value, 5, Properties.RX_PW_P5); }
        }
    }
}
