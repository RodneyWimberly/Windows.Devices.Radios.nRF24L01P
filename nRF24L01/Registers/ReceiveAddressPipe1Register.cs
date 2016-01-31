namespace Windows.Devices.Radios.nRF24L01.Registers
{
    /// <summary>
    ///   Receive address data pipe 1. 5 Bytes maximum length. (LSByte is written first. Write the number of bytes defined by SETUP_AW)
    /// </summary>
    public class ReceiveAddressPipe1Register : RegisterBase

    {
        public ReceiveAddressPipe1Register(Radio radio) : base(radio, 5, Addresses.RX_ADDR_P1)
        {

        }

        public byte[] RX_ADDR_P1
        {
            get { return Value; }
            set { Value = value; }
        }
    }
}
