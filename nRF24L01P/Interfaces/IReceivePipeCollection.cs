using System.Collections.Generic;
using Windows.Devices.Radios.nRF24L01P.Enums;

namespace Windows.Devices.Radios.nRF24L01P.Interfaces
{
    public interface IReceivePipeCollection : IDictionary<int, IReceivePipe>
    {
        FifoStatus FifoStatus { get; }
        void FlushBuffer();
        bool ReceivedPowerDetector { get; }
        byte ReceivePipeNumber { get; }
    }
}
