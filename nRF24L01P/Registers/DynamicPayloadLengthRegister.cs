using Windows.Devices.Radios.nRF24L01P.Interfaces;

namespace Windows.Devices.Radios.nRF24L01P.Registers
{
    /// <summary>
    ///   Enable dynamic payload length
    /// </summary>
    public class DynamicPayloadLengthRegister : RegisterBase
    {
        public DynamicPayloadLengthRegister(ICommandProcessor commandProcessor) : base(commandProcessor, 1, RegisterAddresses.DYNPD)
        {

        }

        public bool DPL_P5
        {
            get { return GetBoolProperty(PropertyMasks.DPL_P5); }
            set { SetBoolProperty(PropertyMasks.DPL_P5, value); }
        }

        public bool DPL_P4
        {
            get { return GetBoolProperty(PropertyMasks.DPL_P4); }
            set { SetBoolProperty(PropertyMasks.DPL_P4, value); }
        }

        public bool DPL_P3
        {
            get { return GetBoolProperty(PropertyMasks.DPL_P3); }
            set { SetBoolProperty(PropertyMasks.DPL_P3, value); }
        }

        public bool DPL_P2
        {
            get { return GetBoolProperty(PropertyMasks.DPL_P2); }
            set { SetBoolProperty(PropertyMasks.DPL_P2, value); }
        }

        public bool DPL_P1
        {
            get { return GetBoolProperty(PropertyMasks.DPL_P1); }
            set { SetBoolProperty(PropertyMasks.DPL_P1, value); }
        }
        public bool DPL_P0
        {
            get { return GetBoolProperty(PropertyMasks.DPL_P0); }
            set { SetBoolProperty(PropertyMasks.DPL_P0, value); }
        }
    }
}
