using Windows.Devices.Radios.nRF24L01P.Interfaces;

namespace Windows.Devices.Radios.nRF24L01P.Registers
{
    public abstract class PipeRegisterBase : RegisterBase
    {
        /// <summary>
        /// Receive Pipe Number
        /// </summary>
        public byte PipeNumber { get; }

        protected PipeRegisterBase(ICommandProcessor commandProcessor, byte address, byte length, byte pipeNumber) :
            base(commandProcessor, length, address)
        {
            PipeNumber = pipeNumber;
        }
    }
}
