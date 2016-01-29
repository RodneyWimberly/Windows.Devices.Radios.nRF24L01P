using System;

namespace Windows.Devices.Radios.nRF24L01.Registers
{
    public class ReceivePipe3AddressRegister : RegisterBase
    {
        public ReceivePipe3AddressRegister(Radio radio) : base(radio, 1, Constants.RX_ADDR_P3)
        {

        }

        public byte RX_ADDR_P3
        {
            get { return FirstByte; }
            set { Value = BitConverter.GetBytes(value); }
        }
    }
}
