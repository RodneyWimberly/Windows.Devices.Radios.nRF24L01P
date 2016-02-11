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

        public bool EN_AA
        {
            get { return Value[0] != 0x0; }
            set { Value[0] = (byte)(value ? 0x3F : 0x0); }
        }

        public bool ENAA_P5
        {
            get { return GetBitValue(Properties.ENAA_P5); }
            set { SetBitValue(Properties.ENAA_P5, value); }
        }

        public bool ENAA_P4
        {
            get { return GetBitValue(Properties.ENAA_P4); }
            set { SetBitValue(Properties.ENAA_P4, value); }
        }

        public bool ENAA_P3
        {
            get { return GetBitValue(Properties.ENAA_P3); }
            set { SetBitValue(Properties.ENAA_P3, value); }
        }

        public bool ENAA_P2
        {
            get { return GetBitValue(Properties.ENAA_P2); }
            set { SetBitValue(Properties.ENAA_P2, value); }
        }

        public bool ENAA_P1
        {
            get { return GetBitValue(Properties.ENAA_P1); }
            set { SetBitValue(Properties.ENAA_P1, value); }
        }

        public bool ENAA_P0
        {
            get { return GetBitValue(Properties.ENAA_P0); }
            set { SetBitValue(Properties.ENAA_P0, value); }
        }
    }
}
