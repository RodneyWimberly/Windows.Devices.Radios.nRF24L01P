using System;

namespace Windows.Devices.Radios.nRF24L01.Registers
{
    public class ReceivePipe2AddressRegister : RegisterBase
    {
        public ReceivePipe2AddressRegister(Radio radio) : base(radio, 1, Constants.RX_ADDR_P2)
        {

        }

        public byte RX_ADDR_P2
        {
            get { return FirstByte; }
            set { Value = BitConverter.GetBytes(value); }
        }
    }
}
