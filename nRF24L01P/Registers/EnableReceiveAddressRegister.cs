using Windows.Devices.Radios.nRF24L01P.Interfaces;

namespace Windows.Devices.Radios.nRF24L01P.Registers
{
    /// <summary>
    ///   Enabled RX Addresses
    /// </summary>
    public class EnableReceiveAddressRegister : RegisterBase
    {
        public EnableReceiveAddressRegister(ICommandProcessor commandProcessor) : base(commandProcessor, 1, RegisterAddresses.EN_RXADDR)
        {

        }

        public bool ERX_P5
        {
            get { return GetBoolProperty(PropertyMasks.ERX_P5); }
            set { SetBoolProperty(PropertyMasks.ERX_P5, value); }
        }

        public bool ERX_P4
        {
            get { return GetBoolProperty(PropertyMasks.ERX_P4); }
            set { SetBoolProperty(PropertyMasks.ERX_P4, value); }
        }

        public bool ERX_P3
        {
            get { return GetBoolProperty(PropertyMasks.ERX_P3); }
            set { SetBoolProperty(PropertyMasks.ERX_P3, value); }
        }

        public bool ERX_P2
        {
            get { return GetBoolProperty(PropertyMasks.ERX_P2); }
            set { SetBoolProperty(PropertyMasks.ERX_P2, value); }
        }

        public bool ERX_P1
        {
            get { return GetBoolProperty(PropertyMasks.ERX_P1); }
            set { SetBoolProperty(PropertyMasks.ERX_P1, value); }
        }

        public bool ERX_P0
        {
            get { return GetBoolProperty(PropertyMasks.ERX_P0); }
            set { SetBoolProperty(PropertyMasks.ERX_P0, value); }
        }
    }
}
