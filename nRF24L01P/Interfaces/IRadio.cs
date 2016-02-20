using Microsoft.IoT.DeviceCore;
using System;
using System.Collections.Generic;
using Windows.Devices.Radios.nRF24L01P.Enums;

namespace Windows.Devices.Radios.nRF24L01P.Interfaces
{
    public interface IRadio : IDevice
    {
        IRegisterContainer RegisterContainer { get; }
        IConfiguration Configuration { get; }
        ITransmitPipe TransmitPipe { get; }
        IDictionary<int, IReceivePipe> ReceivePipes { get; }
        DeviceStatus Status { get; set; }
        string ToString();
        string GetDiagnostics();
        bool ReceivedPowerDetector { get; }

        event EventHandler<InterruptedEventArgs> Interrupted;
    }
}