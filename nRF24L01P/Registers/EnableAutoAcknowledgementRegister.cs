using Windows.Devices.Radios.nRF24L01P.Interfaces;

namespace Windows.Devices.Radios.nRF24L01P.Registers
{
    /// <summary>
    ///   Enable 'Auto Acknowledgment' Function. Disable this functionality to be compatible with nRF2401.
    /// </summary>
    public class EnableAutoAcknowledgementRegister : RegisterBase
    {
        public EnableAutoAcknowledgementRegister(ICommandProcessor commandProcessor) :
            base(commandProcessor, 1, RegisterAddresses.EN_AA)
        { }

        /// <summary>
        /// Enable auto acknowledgement on all pipes
        /// </summary>
        public bool EnableAutoAcknowledgement
        {
            get
            {
                return EnableAutoAcknowledgementPipe0 &&
                       EnableAutoAcknowledgementPipe1 &&
                       EnableAutoAcknowledgementPipe2 &&
                       EnableAutoAcknowledgementPipe3 &&
                       EnableAutoAcknowledgementPipe4 &&
                       EnableAutoAcknowledgementPipe5;
            }
            set
            {
                EnableAutoAcknowledgementPipe0 =
                    EnableAutoAcknowledgementPipe1 =
                    EnableAutoAcknowledgementPipe2 =
                    EnableAutoAcknowledgementPipe3 =
                    EnableAutoAcknowledgementPipe4 =
                    EnableAutoAcknowledgementPipe5 =
                    value;
            }
        }

        /// <summary>
        /// Enable auto acknowledgement data pipe 5 
        /// </summary>
        public bool EnableAutoAcknowledgementPipe5
        {
            get { return GetBoolProperty(PropertyMasks.ENAA_P5); }
            set { SetBoolProperty(PropertyMasks.ENAA_P5, value); }
        }

        /// <summary>
        /// Enable auto acknowledgement data pipe 4
        /// </summary>
        public bool EnableAutoAcknowledgementPipe4
        {
            get { return GetBoolProperty(PropertyMasks.ENAA_P4); }
            set { SetBoolProperty(PropertyMasks.ENAA_P4, value); }
        }

        /// <summary>
        /// Enable auto acknowledgement data pipe 3
        /// </summary>
        public bool EnableAutoAcknowledgementPipe3
        {
            get { return GetBoolProperty(PropertyMasks.ENAA_P3); }
            set { SetBoolProperty(PropertyMasks.ENAA_P3, value); }
        }

        /// <summary>
        /// Enable auto acknowledgement data pipe 2
        /// </summary>
        public bool EnableAutoAcknowledgementPipe2
        {
            get { return GetBoolProperty(PropertyMasks.ENAA_P2); }
            set { SetBoolProperty(PropertyMasks.ENAA_P2, value); }
        }

        /// <summary>
        /// Enable auto acknowledgement data pipe 1
        /// </summary>
        public bool EnableAutoAcknowledgementPipe1
        {
            get { return GetBoolProperty(PropertyMasks.ENAA_P1); }
            set { SetBoolProperty(PropertyMasks.ENAA_P1, value); }
        }

        /// <summary>
        /// Enable auto acknowledgement data pipe 0
        /// </summary>
        public bool EnableAutoAcknowledgementPipe0
        {
            get { return GetBoolProperty(PropertyMasks.ENAA_P0); }
            set { SetBoolProperty(PropertyMasks.ENAA_P0, value); }
        }
    }
}
