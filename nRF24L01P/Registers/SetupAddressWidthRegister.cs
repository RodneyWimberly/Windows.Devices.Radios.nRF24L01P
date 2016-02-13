using Windows.Devices.Radios.nRF24L01P.Interfaces;

namespace Windows.Devices.Radios.nRF24L01P.Registers
{
    /// <summary>
    ///   Setup of Address Widths (common for all data pipes)
    /// </summary>
    public class SetupAddressWidthRegister : RegisterBase
    {
        public SetupAddressWidthRegister(ICommandProcessor commandProcessor) : base(commandProcessor, 1, RegisterAddresses.SETUP_AW)
        {

        }

        public byte AddressWidth
        {
            get { return GetByteProperty(PropertyMasks.AW); }
            set { SetByteProperty(PropertyMasks.AW, value); }
        }
    }
}
