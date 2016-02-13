using Windows.Devices.Radios.nRF24L01P.Interfaces;

namespace Windows.Devices.Radios.nRF24L01P.Registers
{
    /// <summary>
    ///   Enable 'Auto Acknowledgment' Function. Disable this functionality to be compatible with nRF2401.
    /// </summary>
    public class EnableAutoAcknowledgementRegister : RegisterBase
    {
        public EnableAutoAcknowledgementRegister(ICommandProcessor commandProcessor) : base(commandProcessor, 1, RegisterAddresses.EN_AA)
        {

        }

        public bool EnableAutoAcknowledgement
        {
            get { return Value[0] != 0x0; }
            set { Value[0] = (byte)(value ? 0x3F : 0x0); }
        }

        public bool EnableAutoAcknowledgementPipe5
        {
            get { return GetBoolProperty(PropertyMasks.ENAA_P5); }
            set { SetBoolProperty(PropertyMasks.ENAA_P5, value); }
        }

        public bool EnableAutoAcknowledgementPipe4
        {
            get { return GetBoolProperty(PropertyMasks.ENAA_P4); }
            set { SetBoolProperty(PropertyMasks.ENAA_P4, value); }
        }

        public bool EnableAutoAcknowledgementPipe3
        {
            get { return GetBoolProperty(PropertyMasks.ENAA_P3); }
            set { SetBoolProperty(PropertyMasks.ENAA_P3, value); }
        }

        public bool EnableAutoAcknowledgementPipe2
        {
            get { return GetBoolProperty(PropertyMasks.ENAA_P2); }
            set { SetBoolProperty(PropertyMasks.ENAA_P2, value); }
        }

        public bool EnableAutoAcknowledgementPipe1
        {
            get { return GetBoolProperty(PropertyMasks.ENAA_P1); }
            set { SetBoolProperty(PropertyMasks.ENAA_P1, value); }
        }

        public bool EnableAutoAcknowledgementPipe0
        {
            get { return GetBoolProperty(PropertyMasks.ENAA_P0); }
            set { SetBoolProperty(PropertyMasks.ENAA_P0, value); }
        }
    }
}
