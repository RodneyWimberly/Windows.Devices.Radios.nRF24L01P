namespace Windows.Devices.Radios.nRF24L01P.Registers
{
    /// <summary>
    ///   Number of bytes in RX payload in data pipe 1
    /// </summary>
    public class ReceivePayloadWidthPipe1Register : RegisterBase
    {
        public ReceivePayloadWidthPipe1Register(Radio radio) : base(radio, 1, Addresses.RX_PW_P1)
        {

        }

        public byte RX_PW_P1
        {
            get { return GetByteValue(5, Properties.RX_PW_P1); }
            set { SetByteValue(value, 5, Properties.RX_PW_P1); }
        }
    }
}
