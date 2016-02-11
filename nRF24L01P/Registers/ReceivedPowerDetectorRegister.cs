using Windows.Devices.Radios.nRF24L01P.Interfaces;

namespace Windows.Devices.Radios.nRF24L01P.Registers
{
    /// <summary>
    ///   Received Power Detector.
    /// </summary>

    public class ReceivedPowerDetectorRegister : RegisterBase
    {
        public ReceivedPowerDetectorRegister(ICommandProcessor commandProcessor) : base(commandProcessor, 1, RegisterAddresses.RPD)
        {

        }

        public bool RPD => GetBitValue(Properties.RPD);
    }
}
