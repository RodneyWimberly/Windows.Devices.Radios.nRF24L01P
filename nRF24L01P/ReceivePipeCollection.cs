using System.Collections.Generic;
using Windows.Devices.Radios.nRF24L01P.Enums;

namespace Windows.Devices.Radios.nRF24L01P
{
    public class ReceivePipeCollection : Dictionary<byte, ReceivePipe>
    {
        private readonly ICommandProcessor _commandProcessor;
        private readonly Registers.RegisterManager _registers;
        public ReceivePipeCollection(RadioConfiguration configuration, ICommandProcessor commandProcessor)
        {
            _commandProcessor = commandProcessor;
            _registers = configuration.Registers;
            Add(0, new ReceivePipe(configuration, 0));
            Add(1, new ReceivePipe(configuration, 1));
            Add(2, new ReceivePipe(configuration, 2));
            Add(3, new ReceivePipe(configuration, 3));
            Add(4, new ReceivePipe(configuration, 4));
            Add(5, new ReceivePipe(configuration, 5));
        }

        public void FlushBuffer()
        {
            _commandProcessor.ExecuteCommand(DeviceCommands.FLUSH_RX);
        }

        public FifoStatus FifoStatus
        {
            get
            {
                _registers.FifoStatusRegister.Load();
                if (_registers.FifoStatusRegister.RX_FULL)
                    return FifoStatus.Full;
                if (_registers.FifoStatusRegister.RX_EMPTY)
                    return FifoStatus.Empty;
                return FifoStatus.InUse;
            }
        }

    }
}
