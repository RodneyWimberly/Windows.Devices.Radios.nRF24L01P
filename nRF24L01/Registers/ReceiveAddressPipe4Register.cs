namespace Windows.Devices.Radios.nRF24L01.Registers
{
    /// <summary>
    ///   Receive address data pipe 4. 5 Bytes maximum length. (LSByte is written first. Write the number of bytes defined by SETUP_AW)
    /// </summary>
    public class ReceiveAddressPipe4Register : RegisterBase
    {
        public ReceiveAddressPipe4Register(Radio radio) : base(radio, 1, Addresses.RX_ADDR_P4)
        {

        }
    }
}
