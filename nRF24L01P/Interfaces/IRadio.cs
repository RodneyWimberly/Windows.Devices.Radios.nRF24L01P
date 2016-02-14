using System;
using System.Collections.Generic;
using Windows.Devices.Radios.nRF24L01P.Enums;
using Windows.Devices.Radios.nRF24L01P.Registers;

namespace Windows.Devices.Radios.nRF24L01P.Interfaces
{
    public interface IRadio
    {
        ICommandProcessor CommandProcessor { get; }
        IRadioConfiguration Configuration { get; }
        ITransmitPipe TransmitPipe { get; }
        IDictionary<int, IReceivePipe> ReceivePipes { get; }
        bool ReceivedPowerDetector { get; }
        DeviceStatus Status { get; set; }
        string ToString();
        string GetDiagnostics();
        void Initialize();
        void ChipEnable(bool enabled);
        event EventHandler<StatusRegister> OnInterrupt;
    }
}