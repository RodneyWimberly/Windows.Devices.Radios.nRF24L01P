namespace Windows.Devices.Radios.nRF24L01P.Registers
{
    /// <summary>
    ///   Transmit address. Used for a PTX device only. (LSByte is written first) 
    ///   Set RX_ADDR_P0 equal to this address to handle automatic acknowledge if this is a PTX device with Enhanced ShockBurst™ enabled.
    /// </summary>
    public class TransmitAddressRegister : RegisterBase
    {
        public TransmitAddressRegister(Radio radio) : base(radio, 5, Addresses.TX_ADDR)
        {
        }
    }
}
