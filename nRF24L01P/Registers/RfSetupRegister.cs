using Windows.Devices.Radios.nRF24L01P.Interfaces;

namespace Windows.Devices.Radios.nRF24L01P.Registers
{
    /// <summary>
    ///   RF Setup Register
    /// </summary>
    public class RfSetupRegister : RegisterBase
    {
        public RfSetupRegister(ICommandProcessor commandProcessor) : base(commandProcessor, 1, RegisterAddresses.RF_SETUP)
        {

        }
        public bool EnableContiuousCarrierTransmit
        {
            get { return GetBoolProperty(PropertyMasks.CONT_WAVE); }
            set { SetBoolProperty(PropertyMasks.CONT_WAVE, value); }
        }

        public bool DataRateLow
        {
            get { return GetBoolProperty(PropertyMasks.RF_DR_LOW); }
            set { SetBoolProperty(PropertyMasks.RF_DR_LOW, value); }
        }

        public bool ForcePllSignalLock
        {
            get { return GetBoolProperty(PropertyMasks.PLL_LOCK); }
            set { SetBoolProperty(PropertyMasks.PLL_LOCK, value); }
        }

        public bool DataRateHigh
        {
            get { return GetBoolProperty(PropertyMasks.RF_DR_HIGH); }
            set { SetBoolProperty(PropertyMasks.RF_DR_HIGH, value); }
        }

        public bool PowerLevelHigh
        {
            get { return GetBoolProperty(PropertyMasks.RF_PWR_HIGH); }
            set { SetBoolProperty(PropertyMasks.RF_PWR_HIGH, value); }
        }

        public bool PowerLevelLow
        {
            get { return GetBoolProperty(PropertyMasks.RF_PWR_LOW); }
            set { SetBoolProperty(PropertyMasks.RF_PWR_LOW, value); }
        }
    }
}
