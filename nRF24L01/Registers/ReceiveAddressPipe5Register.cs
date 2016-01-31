namespace Windows.Devices.Radios.nRF24L01P.Registers
{
    public class ReceiveAddressPipe5Register : RegisterBase
    {
        /// <summary>
        ///   Receive address data pipe 5. 5 Bytes maximum length. (LSByte is written first. Write the number of bytes defined by SETUP_AW)
        /// </summary>
        public ReceiveAddressPipe5Register(Radio radio) : base(radio, 1, Addresses.RX_ADDR_P5)
        {

        }

    }
}
