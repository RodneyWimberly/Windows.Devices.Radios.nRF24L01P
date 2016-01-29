namespace Windows.Devices.Radios.nRF24L01.Registers
{
    public class ReceivedPowerDetectorRegister : RegisterBase
    {
        public ReceivedPowerDetectorRegister(Radio radio) : base(radio, 1, Constants.RPD)
        {

        }

        public bool RPD => (FirstByte & 1) == 1;
    }
}
