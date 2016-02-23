using System.Collections.Generic;
using Windows.Devices.Radios.nRF24L01P.Enums;
using Windows.Devices.Radios.nRF24L01P.Interfaces;

namespace Windows.Devices.Radios.nRF24L01P
{
    public class ReceivePipeCollection : Dictionary<int, IReceivePipe>, IReceivePipeCollection
    {
        private readonly IRegisterContainer _registerContainer;
        private readonly ICommandProcessor _commandProcessor;

        public ReceivePipeCollection(IConfiguration configuration, ICommandProcessor commandProcessor, IRegisterContainer registerContainer)
        {
            _registerContainer = registerContainer;
            _commandProcessor = commandProcessor;
            Add(0, new ReceivePipe(configuration, commandProcessor, registerContainer, this, 0));
            Add(1, new ReceivePipe(configuration, commandProcessor, registerContainer, this, 1));
            Add(2, new ReceivePipe(configuration, commandProcessor, registerContainer, this, 2));
            Add(3, new ReceivePipe(configuration, commandProcessor, registerContainer, this, 3));
            Add(4, new ReceivePipe(configuration, commandProcessor, registerContainer, this, 4));
            Add(5, new ReceivePipe(configuration, commandProcessor, registerContainer, this, 5));
        }

        public bool ReceivedPowerDetector
        {
            get
            {
                _registerContainer.ReceivedPowerDetectorRegister.Load();
                return _registerContainer.ReceivedPowerDetectorRegister.ReceivedPowerDetector;
            }
        }


        public FifoStatus FifoStatus
        {
            get
            {
                FifoStatus fifoStatus = FifoStatus.InUse;
                _registerContainer.FifoStatusRegister.Load();
                if (_registerContainer.FifoStatusRegister.ReceiveFifoFull)
                    fifoStatus = FifoStatus.Full;
                else if (_registerContainer.FifoStatusRegister.ReveiveFifoEmpty)
                    fifoStatus = FifoStatus.Empty;
                return fifoStatus;
            }
        }

        public void FlushBuffer()
        {
            _commandProcessor.ExecuteCommand(DeviceCommands.FLUSH_RX);
        }
    }
}
