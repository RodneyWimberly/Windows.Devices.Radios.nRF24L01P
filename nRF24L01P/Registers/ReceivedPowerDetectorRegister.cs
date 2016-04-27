using Common.Logging;
using Windows.Devices.Radios.nRF24L01P.Interfaces;

namespace Windows.Devices.Radios.nRF24L01P.Registers
{
    /// <summary>
    ///   Received Power Detector.
    /// </summary>

    public class ReceivedPowerDetectorRegister : RegisterBase
    {
        public ReceivedPowerDetectorRegister(ILoggerFactoryAdapter loggerFactoryAdapter, ICommandProcessor commandProcessor) :
            base(loggerFactoryAdapter, commandProcessor, 1, RegisterAddresses.RPD, RegisterDefaults.RPD, "RPD")
        { }

        /// <summary>
        /// Received Power Detector. This register is called 
        /// CD(Carrier Detect) in the nRF24L01.The name is 
        /// different in nRF24L01+ due to the different input
        /// power level threshold for this bit.
        /// </summary>
        public bool ReceivedPowerDetector => GetBoolProperty(PropertyMasks.RPD);
    }
}
