using Windows.Devices.Radios.nRF24L01P.Interfaces;

namespace Windows.Devices.Radios.nRF24L01P.Registers
{
    /// <summary>
    ///   Setup of Address Widths (common for all data pipes)
    /// </summary>
    public class SetupAddressWidthRegister : RegisterBase
    {
        public SetupAddressWidthRegister(ICommandProcessor commandProcessor) :
            base(commandProcessor, 1, RegisterAddresses.SETUP_AW)
        { }

        /// <summary>
        /// RX/TX Address field width 
        /// '00' - Illegal 
        /// '01' - 3 bytes 
        /// '10' - 4 bytes 
        /// '11' - 5 bytes
        /// LSByte is used if address width is below 5 bytes
        /// </summary>
        public byte AddressWidth
        {
            get { return GetByteProperty(PropertyMasks.AW); }
            set { SetByteProperty(PropertyMasks.AW, value); }
        }
    }
}
