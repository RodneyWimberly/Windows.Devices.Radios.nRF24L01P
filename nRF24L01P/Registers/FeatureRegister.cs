using Windows.Devices.Radios.nRF24L01P.Interfaces;

namespace Windows.Devices.Radios.nRF24L01P.Registers
{
    /// <summary>
    ///   Feature Register
    /// </summary>
    public class FeatureRegister : RegisterBase
    {
        public FeatureRegister(ICommandProcessor commandProcessor) : base(commandProcessor, 1, RegisterAddresses.FEATURE)
        {

        }

        public bool EN_DPL
        {
            get { return GetBitValue(Properties.EN_DPL); }
            set { SetBitValue(Properties.EN_DPL, value); }
        }

        public bool EN_ACK_PAY
        {
            get { return GetBitValue(Properties.EN_ACK_PAY); }
            set { SetBitValue(Properties.EN_ACK_PAY, value); }
        }

        public bool EN_DYN_ACK
        {
            get { return GetBitValue(Properties.EN_DYN_ACK); }
            set { SetBitValue(Properties.EN_DYN_ACK, value); }
        }

    }
}
