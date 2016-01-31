namespace Windows.Devices.Radios.nRF24L01P.Registers
{
    /// <summary>
    ///   Number of bytes in RX payload in data pipe 3
    /// </summary>
    public class ReceivePayloadWidthPipe3Register : RegisterBase
    {
        public ReceivePayloadWidthPipe3Register(Radio radio) : base(radio, 1, Addresses.RX_PW_P3)
        {

        }

        public byte RX_PW_P3
        {
            get { return GetByteValue(5, Properties.RX_PW_P3); }
            set { SetByteValue(value, 5, Properties.RX_PW_P3); }
        }
    }
}
