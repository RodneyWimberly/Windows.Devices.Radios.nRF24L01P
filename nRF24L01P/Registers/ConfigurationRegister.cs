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

        public bool MASK_RX_DR
        {
            get { return GetBoolProperty(PropertyMasks.MASK_RX_DR); }
            set { SetBoolProperty(PropertyMasks.MASK_RX_DR, value); }
        }

        public bool MASK_TX_DS
        {
            get { return GetBoolProperty(PropertyMasks.MASK_TX_DS); }
            set { SetBoolProperty(PropertyMasks.MASK_TX_DS, value); }
        }

        public bool MASK_MAX_RT
        {
            get { return GetBoolProperty(PropertyMasks.MASK_MAX_RT); }
            set { SetBoolProperty(PropertyMasks.MASK_MAX_RT, value); }
        }

        public bool EN_CRC
        {
            get { return GetBoolProperty(PropertyMasks.EN_CRC); }
            set { SetBoolProperty(PropertyMasks.EN_CRC, value); }
        }

        public bool CRCO
        {
            get { return GetBoolProperty(PropertyMasks.CRCO); }
            set { SetBoolProperty(PropertyMasks.CRCO, value); }
        }

        public bool PWR_UP
        {
            get { return GetBoolProperty(PropertyMasks.PWR_UP); }
            set { SetBoolProperty(PropertyMasks.PWR_UP, value); }
        }

        public bool PRIM_RX
        {
            get { return GetBoolProperty(PropertyMasks.PRIM_RX); }
            set { SetBoolProperty(PropertyMasks.PRIM_RX, value); }
        }
    }
}
