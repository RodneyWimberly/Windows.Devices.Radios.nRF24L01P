namespace Windows.Devices.Radios.nRF24L01P.Registers
{
    /// <summary>
    ///   Received Power Detector.
    /// </summary>

    public class ReceivedPowerDetectorRegister : RegisterBase
    {
        public ReceivedPowerDetectorRegister(Radio radio) : base(radio, 1, Addresses.RPD)
        {

        }

        public bool RPD => GetBitValue(Properties.RPD);
    }
}
