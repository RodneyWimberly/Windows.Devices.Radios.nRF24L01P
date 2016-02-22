using Windows.Devices.Radios.nRF24L01P.Interfaces;

namespace Windows.Devices.Radios.nRF24L01P.Registers
{
    /// <summary>
    ///   RF Setup Register
    /// </summary>
    public class RfSetupRegister : RegisterBase
    {
        public RfSetupRegister(ICommandProcessor commandProcessor) :
            base(commandProcessor, 1, RegisterAddresses.RF_SETUP, RegisterDefaults.RF_SETUP, "RF_SETUP")
        { }

        /// <summary>
        /// Enables continuous carrier transmit when high. 
        /// Only '0' allowed
        /// </summary>
        public bool EnableContiuousCarrierTransmit
        {
            get { return GetBoolProperty(PropertyMasks.CONT_WAVE); }
            set { SetBoolProperty(PropertyMasks.CONT_WAVE, value); }
        }

        /// <summary>
        /// Force PLL lock signal Only used in test 
        /// </summary>
        public bool ForcePllSignalLock
        {
            get { return GetBoolProperty(PropertyMasks.PLL_LOCK); }
            set { SetBoolProperty(PropertyMasks.PLL_LOCK, value); }
        }

        /// <summary>
        /// DataRate Low Bit
        /// Set RF Data Rate to 250kbps. 
        /// See RF DR HIGH for encoding.
        /// </summary>
        public bool DataRateLow
        {
            get { return GetBoolProperty(PropertyMasks.RF_DR_LOW); }
            set { SetBoolProperty(PropertyMasks.RF_DR_LOW, value); }
        }

        /// <summary>
        /// DataRate High Bit
        /// Select between the high speed data rates. This bit 
        /// is don't care if DataRateLow is set. 
        /// Encoding: 
        /// (DataRateLow, DataRateHigh): 
        /// '00' — 1Mbps 
        /// '01' - 2Mbps 
        /// '10' - 250kbps 
        /// '11' — Reserved
        /// </summary>
        public bool DataRateHigh
        {
            get { return GetBoolProperty(PropertyMasks.RF_DR_HIGH); }
            set { SetBoolProperty(PropertyMasks.RF_DR_HIGH, value); }
        }

        /// <summary>
        /// Set RF output power in TX mode
        ///' 00' = -18dBm 
        /// '01' = -12dBm
        /// '10' = -6dBm
        /// '11' = OdBm
        /// </summary>
        public byte PowerLevel
        {
            get { return GetByteProperty(PropertyMasks.RF_PWR); }
            set { SetByteProperty(PropertyMasks.RF_PWR, value); }
        }
    }
}
