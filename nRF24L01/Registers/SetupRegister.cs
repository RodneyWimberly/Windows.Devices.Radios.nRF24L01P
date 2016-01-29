namespace Windows.Devices.Radios.nRF24L01.Registers
{
    public class SetupRegister : RegisterBase
    {
        public SetupRegister(Radio radio) : base(radio, 1, Constants.RF_SETUP)
        {

        }
        public bool CONT_WAVE
        {
            get { return GetBitValue(Constants.CONT_WAVE); }
            set { FirstByte = SetBitValue(Constants.CONT_WAVE, value); }
        }

        public bool RF_DR_LOW
        {
            get { return GetBitValue(Constants.RF_DR_LOW); }
            set { FirstByte = SetBitValue(Constants.RF_DR_LOW, value); }
        }

        public bool PLL_LOCK
        {
            get { return GetBitValue(Constants.PLL_LOCK); }
            set { FirstByte = SetBitValue(Constants.PLL_LOCK, value); }
        }

        public bool RF_DR_HIGH
        {
            get { return GetBitValue(Constants.RF_DR_HIGH); }
            set { FirstByte = SetBitValue(Constants.RF_DR_HIGH, value); }
        }

        public byte RF_PWR
        {
            get { return GetByteValue(2, 1); }
            set { FirstByte = SetByteValue(value, 2, 1); }
        }
    }
}
