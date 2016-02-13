using System.Collections.Generic;
using Windows.Devices.Radios.nRF24L01P.Enums;

namespace Windows.Devices.Radios.nRF24L01P.Interfaces
{
    public interface IRadio
    {
        ICommandProcessor CommandProcessor { get; }
        IRadioConfiguration Configuration { get; }
        ITransmitPipe TransmitPipe { get; }
        IDictionary<int, IReceivePipe> ReceivePipes { get; }
        bool ChannelReceivedPowerDetector { get; }
        DeviceStatus Status { get; set; }
        string ToString();
        void Initialize();
        void ChipEnable(bool enabled);
    }
}