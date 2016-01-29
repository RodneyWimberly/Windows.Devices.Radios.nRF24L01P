using System.Collections.Generic;
using Windows.Devices.Radios.nRF24L01.Enums;

namespace Windows.Devices.Radios.nRF24L01
{
    public class ReceivePipeCollection : Dictionary<byte, ReceivePipe>
    {
        private readonly Radio _radio;
        private readonly Registers.RegisterCollection _registers;
        public ReceivePipeCollection(Radio radio)
        {
            _radio = radio;
            _registers = _radio.Configuration.Registers;
            Add(0, new ReceivePipe(radio, 0));
            Add(1, new ReceivePipe(radio, 1));
            Add(2, new ReceivePipe(radio, 2));
            Add(3, new ReceivePipe(radio, 3));
            Add(4, new ReceivePipe(radio, 4));
            Add(5, new ReceivePipe(radio, 5));
        }

        public void FlushBuffer()
        {
            _radio.Transfer(Constants.FLUSH_RX);
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
