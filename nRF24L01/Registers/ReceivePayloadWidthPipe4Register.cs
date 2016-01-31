namespace Windows.Devices.Radios.nRF24L01P.Registers
{
    /// <summary>
    ///   Number of bytes in RX payload in data pipe 4
    /// </summary>
    public class ReceivePayloadWidthPipe4Register : RegisterBase
    {
        public ReceivePayloadWidthPipe4Register(Radio radio) : base(radio, 1, Addresses.RX_PW_P4)
        {

        }

        public byte RX_PW_P4
        {
            get { return GetByteValue(5, Properties.RX_PW_P4); }
            set { SetByteValue(value, 5, Properties.RX_PW_P4); }
        }
    }
}
