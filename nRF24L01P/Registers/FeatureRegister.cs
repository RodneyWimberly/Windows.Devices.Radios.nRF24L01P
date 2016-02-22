using Windows.Devices.Radios.nRF24L01P.Interfaces;

namespace Windows.Devices.Radios.nRF24L01P.Registers
{
    /// <summary>
    ///   Feature Register
    /// </summary>
    public class FeatureRegister : RegisterBase
    {
        public FeatureRegister(ICommandProcessor commandProcessor) :
            base(commandProcessor, 1, RegisterAddresses.FEATURE, RegisterDefaults.FEATURE, "FEATURE")
        { }

        /// <summary>
        /// Enables Dynamic Payload Length
        /// </summary>
        public bool EnableDynamicPayloadLength
        {
            get { return GetBoolProperty(PropertyMasks.EN_DPL); }
            set { SetBoolProperty(PropertyMasks.EN_DPL, value); }
        }

        /// <summary>
        /// Enables Payload with ACK 
        /// </summary>
        public bool EnablePayloadWithAcknowledgement
        {
            get { return GetBoolProperty(PropertyMasks.EN_ACK_PAY); }
            set { SetBoolProperty(PropertyMasks.EN_ACK_PAY, value); }
        }

        /// <summary>
        /// Enables thew TX PAYLOAD NOACK command 
        /// </summary>
        public bool EnableTransmitWithPayloadAcknowledgement
        {
            get { return GetBoolProperty(PropertyMasks.EN_DYN_ACK); }
            set { SetBoolProperty(PropertyMasks.EN_DYN_ACK, value); }
        }
    }
}
