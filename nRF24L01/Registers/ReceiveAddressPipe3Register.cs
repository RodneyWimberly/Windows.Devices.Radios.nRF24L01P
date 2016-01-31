using System;

namespace Windows.Devices.Radios.nRF24L01.Registers
{
    /// <summary>
    ///   Receive address data pipe 3. 5 Bytes maximum length. (LSByte is written first. Write the number of bytes defined by SETUP_AW)
    /// </summary>
    public class ReceiveAddressPipe3Register : RegisterBase
    {
        public ReceiveAddressPipe3Register(Radio radio) : base(radio, 1, Addresses.RX_ADDR_P3)
        {

        }

        public byte RX_ADDR_P3
        {
            get { return FirstByte; }
            set { Value = BitConverter.GetBytes(value); }
        }
    }
}
