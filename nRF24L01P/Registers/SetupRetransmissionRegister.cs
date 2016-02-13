using Windows.Devices.Radios.nRF24L01P.Interfaces;

namespace Windows.Devices.Radios.nRF24L01P.Registers
{
    /// <summary>
    ///   Setup of Automatic Retransmission
    /// </summary>
    public class SetupRetransmissionRegister : RegisterBase
    {
        public SetupRetransmissionRegister(ICommandProcessor commandProcessor) : base(commandProcessor, 1, RegisterAddresses.SETUP_RETR)
        {

        }

        public byte AutoRetransmitDelay
        {
            get { return GetByteProperty(PropertyMasks.ARD); }
            set { SetByteProperty(PropertyMasks.ARD, value); }
        }

        public byte AutoRetransmitCount
        {
            get { return GetByteProperty(PropertyMasks.ARC); }
            set { SetByteProperty(PropertyMasks.ARC, value); }
        }
    }
}
