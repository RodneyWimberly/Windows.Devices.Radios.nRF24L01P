using Windows.Devices.Radios.nRF24L01P.Interfaces;

namespace Windows.Devices.Radios.nRF24L01P.Registers
{
    /// <summary>
    ///   Configuration Register
    /// </summary>
    public class ConfigurationRegister : RegisterBase
    {
        public ConfigurationRegister(ICommandProcessor commandProcessor) :
            base(commandProcessor, 1, RegisterAddresses.CONFIG)
        { }

        /// <summary>
        /// Mask interrupt caused by RX DR 
        /// 1: Interrupt not reflected on the IRQ pin 
        /// 0: Reflect RX DR as active low interrupt on the
        /// IRQ pin
        /// </summary>
        public bool ReceiveDataReadyMask
        {
            get { return GetBoolProperty(PropertyMasks.MASK_RX_DR); }
            set { SetBoolProperty(PropertyMasks.MASK_RX_DR, value); }
        }

        /// <summary>
        /// Mask interrupt caused by TX DS 
        /// 1: Interrupt not reflected on the IRQ pin 
        /// 0: Reflect TX DS as active low interrupt on the IRQ
        /// Pin
        /// </summary>
        public bool TransmitDataSentMask
        {
            get { return GetBoolProperty(PropertyMasks.MASK_TX_DS); }
            set { SetBoolProperty(PropertyMasks.MASK_TX_DS, value); }
        }

        /// <summary>
        /// Mask interrupt caused by RT 
        /// 1: Interrupt not reflected on the IRQ pin 
        /// 0: Reflect PLAX RT as active low interrupt on the
        /// IRQ pin
        /// </summary>
        public bool MaximunTransmitRetriesMask
        {
            get { return GetBoolProperty(PropertyMasks.MASK_MAX_RT); }
            set { SetBoolProperty(PropertyMasks.MASK_MAX_RT, value); }
        }

        /// <summary>
        /// Enable CRC. Forced high if one of the bits in the 
        /// EN is high
        /// </summary>
        public bool EnableCrc
        {
            get { return GetBoolProperty(PropertyMasks.EN_CRC); }
            set { SetBoolProperty(PropertyMasks.EN_CRC, value); }
        }

        /// <summary>
        /// CRC encoding scheme 
        /// '0' - 1 byte 
        /// '1' — 2 bytes
        /// </summary>
        public bool CrcEncodingScheme
        {
            get { return GetBoolProperty(PropertyMasks.CRCO); }
            set { SetBoolProperty(PropertyMasks.CRCO, value); }
        }

        /// <summary>
        /// 1: POWER up, O:POWER DOWN 
        /// </summary>
        public bool PowerUp
        {
            get { return GetBoolProperty(PropertyMasks.PWR_UP); }
            set { SetBoolProperty(PropertyMasks.PWR_UP, value); }
        }

        /// <summary>
        /// RX/TX control 
        /// 1: O: PTX
        /// </summary>
        public bool PrimaryReceiveMode
        {
            get { return GetBoolProperty(PropertyMasks.PRIM_RX); }
            set { SetBoolProperty(PropertyMasks.PRIM_RX, value); }
        }
    }
}
