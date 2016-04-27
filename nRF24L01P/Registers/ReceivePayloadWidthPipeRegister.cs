using Common.Logging;
using Windows.Devices.Radios.nRF24L01P.Interfaces;

namespace Windows.Devices.Radios.nRF24L01P.Registers
{
    /// <summary>
    ///   Number of bytes in RX payload in data pipe 
    /// </summary>
    public class ReceivePayloadWidthPipeRegister : PipeRegisterBase
    {
        public ReceivePayloadWidthPipeRegister(ILoggerFactoryAdapter loggerFactoryAdapter, ICommandProcessor commandProcessor, byte address, byte[] defaultValue, byte pipeNumber, string name = "") :
            base(loggerFactoryAdapter, commandProcessor, address, 1, defaultValue, pipeNumber, name)
        { }

        /// <summary>
        /// Number of bytes in RX payload in data pipe x (1 to 32 bytes)
        /// 0 Pipe not used 
        /// 1 = 1 byte 
        /// ...
        /// 32 32 bytes
        /// </summary>
        public byte PayloadWidth
        {
            get { return GetByteProperty(PropertyMasks.RX_PW_P0); }
            set { SetByteProperty(PropertyMasks.RX_PW_P0, value); }
        }
    }
}
