using Windows.Devices.Radios.nRF24L01P.Interfaces;

namespace Windows.Devices.Radios.nRF24L01P.Registers
{
    /// <summary>
    ///   RF Channel
    /// </summary>
    public class RfChannelRegister : RegisterBase
    {
        public RfChannelRegister(ICommandProcessor commandProcessor) :
            base(commandProcessor, 1, RegisterAddresses.RF_CH, RegisterDefaults.RF_CH, "RF_CH")
        { }

        /// <summary>
        /// Sets the frequency channel nRF24L01+ operates on
        /// </summary>
        public byte RfChannel
        {
            get { return GetByteProperty(PropertyMasks.RF_CH); }
            set { SetByteProperty(PropertyMasks.RF_CH, value); }
        }
    }
}
