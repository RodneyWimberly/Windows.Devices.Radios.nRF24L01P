namespace Windows.Devices.Radios.nRF24L01.Registers
{
    /// <summary>
    ///   Receive address data pipe 2. 5 Bytes maximum length. (LSByte is written first. Write the number of bytes defined by SETUP_AW)
    /// </summary>
    public class ReceiveAddressPipe2Register : RegisterBase
    {
        public ReceiveAddressPipe2Register(Radio radio) : base(radio, 1, Addresses.RX_ADDR_P2)
        {

        }

    }
}
