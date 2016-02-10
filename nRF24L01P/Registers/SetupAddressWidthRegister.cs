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

        public byte AW
        {
            get { return GetByteValue(Properties.AW, 0); }
            set { SetByteValue(value, Properties.AW, 0); }
        }
    }
}
