namespace Windows.Devices.Radios.nRF24L01.Registers
{
    /// <summary>
    ///   Receive address data pipe 0. 5 Bytes maximum length. (LSByte is written first. Write the number of bytes defined by SETUP_AW)
    /// </summary>
    public class ReceiveAddressPipe0Register : RegisterBase
    {
        public ReceiveAddressPipe0Register(Radio radio) : base(radio, 5, Addresses.RX_ADDR_P0)
        {

        }
    }
}
