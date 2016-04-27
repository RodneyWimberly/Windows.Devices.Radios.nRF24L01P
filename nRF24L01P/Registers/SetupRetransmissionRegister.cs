using Common.Logging;
using Windows.Devices.Radios.nRF24L01P.Interfaces;

namespace Windows.Devices.Radios.nRF24L01P.Registers
{
    /// <summary>
    ///   Setup of Automatic Retransmission
    /// </summary>
    public class SetupRetransmissionRegister : RegisterBase
    {
        public SetupRetransmissionRegister(ILoggerFactoryAdapter loggerFactoryAdapter, ICommandProcessor commandProcessor) :
            base(loggerFactoryAdapter, commandProcessor, 1, RegisterAddresses.SETUP_RETR, RegisterDefaults.SETUP_RETR, "SETUP_RETR")
        { }

        /// <summary>
        /// Setup of Automatic Retransmission 
        /// Auto Retransmit Delay 
        /// '0000' - Wait 250VS 
        /// '0001' - Wait 
        /// '0010' - Wait 7501-IS
        /// (Delay defined from end of transmission to start of next transmission)
        /// </summary>
        public byte AutoRetransmitDelay
        {
            get { return GetByteProperty(PropertyMasks.ARD); }
            set { SetByteProperty(PropertyMasks.ARD, value); }
        }

        /// <summary>
        /// Auto Retransmit Count 
        /// '0000' — Re-Transmit disabled 
        /// '0001' — Up to 1 Re-Transmit on fail of AA
        /// </summary>
        public byte AutoRetransmitCount
        {
            get { return GetByteProperty(PropertyMasks.ARC); }
            set { SetByteProperty(PropertyMasks.ARC, value); }
        }
    }
}
