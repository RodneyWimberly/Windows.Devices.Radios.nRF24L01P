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

        /// <summary>
        /// Enable dynamic payload length data pipe 5. 
        /// (Requires EnableDynamicPayloadLength and EnableAutoAcknowledgementPipe5) 
        /// </summary>
        public bool EnableDynamicPayloadPipe5
        {
            get { return GetBoolProperty(PropertyMasks.DPL_P5); }
            set { SetBoolProperty(PropertyMasks.DPL_P5, value); }
        }

        /// <summary>
        /// Enable dynamic payload length data pipe 4. 
        /// (Requires EnableDynamicPayloadLength and EnableAutoAcknowledgementPipe4) 
        /// </summary>
        public bool EnableDynamicPayloadPipe4
        {
            get { return GetBoolProperty(PropertyMasks.DPL_P4); }
            set { SetBoolProperty(PropertyMasks.DPL_P4, value); }
        }

        /// <summary>
        /// Enable dynamic payload length data pipe 3. 
        /// (Requires EnableDynamicPayloadLength and EnableAutoAcknowledgementPipe3) 
        /// </summary>
        public bool EnableDynamicPayloadPipe3
        {
            get { return GetBoolProperty(PropertyMasks.DPL_P3); }
            set { SetBoolProperty(PropertyMasks.DPL_P3, value); }
        }

        /// <summary>
        /// Enable dynamic payload length data pipe 2. 
        /// (Requires EnableDynamicPayloadLength and EnableAutoAcknowledgementPipe2) 
        /// </summary>
        public bool EnableDynamicPayloadPipe2
        {
            get { return GetBoolProperty(PropertyMasks.DPL_P2); }
            set { SetBoolProperty(PropertyMasks.DPL_P2, value); }
        }

        /// <summary>
        /// Enable dynamic payload length data pipe 1. 
        /// (Requires EnableDynamicPayloadLength and EnableAutoAcknowledgementPipe1) 
        /// </summary>
        public bool EnableDynamicPayloadPipe1
        {
            get { return GetBoolProperty(PropertyMasks.DPL_P1); }
            set { SetBoolProperty(PropertyMasks.DPL_P1, value); }
        }

        /// <summary>
        /// Enable dynamic payload length data pipe 0. 
        /// (Requires EnableDynamicPayloadLength and EnableAutoAcknowledgementPipe0) 
        /// </summary>
        public bool EnableDynamicPayloadPipe0
        {
            get { return GetBoolProperty(PropertyMasks.DPL_P0); }
            set { SetBoolProperty(PropertyMasks.DPL_P0, value); }
        }
    }
}
