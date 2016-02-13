using Windows.Devices.Radios.nRF24L01P.Interfaces;

namespace Windows.Devices.Radios.nRF24L01P.Registers
{
    /// <summary>
    ///   Configuration Register
    /// </summary>
    public class ConfigurationRegister : RegisterBase
    {
        public ConfigurationRegister(ICommandProcessor commandProcessor) : base(commandProcessor, 1, RegisterAddresses.CONFIG)
        {

        }

        public bool ReceiveDataReadyMask
        {
            get { return GetBoolProperty(PropertyMasks.MASK_RX_DR); }
            set { SetBoolProperty(PropertyMasks.MASK_RX_DR, value); }
        }

        public bool TransmitDataSentMask
        {
            get { return GetBoolProperty(PropertyMasks.MASK_TX_DS); }
            set { SetBoolProperty(PropertyMasks.MASK_TX_DS, value); }
        }

        public bool MaximunTransmitRetriesMask
        {
            get { return GetBoolProperty(PropertyMasks.MASK_MAX_RT); }
            set { SetBoolProperty(PropertyMasks.MASK_MAX_RT, value); }
        }

        public bool EnableCrc
        {
            get { return GetBoolProperty(PropertyMasks.EN_CRC); }
            set { SetBoolProperty(PropertyMasks.EN_CRC, value); }
        }

        public bool CrcEncodingScheme
        {
            get { return GetBoolProperty(PropertyMasks.CRCO); }
            set { SetBoolProperty(PropertyMasks.CRCO, value); }
        }

        public bool PowerUp
        {
            get { return GetBoolProperty(PropertyMasks.PWR_UP); }
            set { SetBoolProperty(PropertyMasks.PWR_UP, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool PrimaryReceiveMode
        {
            get { return GetBoolProperty(PropertyMasks.PRIM_RX); }
            set { SetBoolProperty(PropertyMasks.PRIM_RX, value); }
        }
    }
}
