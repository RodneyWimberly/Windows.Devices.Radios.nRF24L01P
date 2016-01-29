using System;

namespace Windows.Devices.Radios.nRF24L01.Registers
{
    public class ReceivePipe5AddressRegister : RegisterBase
    {
        public ReceivePipe5AddressRegister(Radio radio) : base(radio, 1, Constants.RX_ADDR_P5)
        {

        }
        public byte RX_ADDR_P5
        {
            get { return FirstByte; }
            set { Value = BitConverter.GetBytes(value); }
        }
    }
}
