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

        public byte ARD
        {
            get { return GetByteProperty(PropertyMasks.ARD); }
            set { SetByteProperty(PropertyMasks.ARD, value); }
        }

        public byte ARC
        {
            get { return GetByteProperty(PropertyMasks.ARC); }
            set { SetByteProperty(PropertyMasks.ARC, value); }
        }
    }
}
