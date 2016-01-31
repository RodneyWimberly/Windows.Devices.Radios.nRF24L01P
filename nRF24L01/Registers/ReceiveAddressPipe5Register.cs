using System;

namespace Windows.Devices.Radios.nRF24L01.Registers
{
    public class ReceiveAddressPipe5Register : RegisterBase
    {
        /// <summary>
        ///   Receive address data pipe 5. 5 Bytes maximum length. (LSByte is written first. Write the number of bytes defined by SETUP_AW)
        /// </summary>
        public ReceiveAddressPipe5Register(Radio radio) : base(radio, 1, Addresses.RX_ADDR_P5)
        {

        }
        public byte RX_ADDR_P5
        {
            get { return FirstByte; }
            set { Value = BitConverter.GetBytes(value); }
        }
    }
}
