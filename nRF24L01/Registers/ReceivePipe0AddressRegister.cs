using System;

namespace Windows.Devices.Radios.nRF24L01.Registers
{
    public class ReceivePipe0AddressRegister : RegisterBase
    {
        public ReceivePipe0AddressRegister(Radio radio) : base(radio, 5, Constants.RX_ADDR_P0)
        {

        }

        public byte[] RX_ADDR_P0
        {
            get { return Value; }
            set { Value = value; }
        }
    }
}
