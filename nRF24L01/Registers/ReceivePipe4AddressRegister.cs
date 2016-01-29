using System;

namespace Windows.Devices.Radios.nRF24L01.Registers
{
    public class ReceivePipe4AddressRegister : RegisterBase
    {
        public ReceivePipe4AddressRegister(Radio radio) : base(radio, 1, Constants.RX_ADDR_P4)
        {

        }

        public byte RX_ADDR_P4
        {
            get { return FirstByte; }
            set { Value = BitConverter.GetBytes(value); }
        }
    }
}
