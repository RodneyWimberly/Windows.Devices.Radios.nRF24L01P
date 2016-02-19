using System;
using System.Collections.Generic;
using Windows.Devices.Radios.nRF24L01P.Enums;
using Windows.Devices.Radios.nRF24L01P.Registers;

namespace Windows.Devices.Radios.nRF24L01P.Interfaces
{
    public interface IRadio
    {
        IRegisterManager RegisterManager { get; }
        IRadioConfiguration Configuration { get; }
        ITransmitPipe TransmitPipe { get; }
        IDictionary<int, IReceivePipe> ReceivePipes { get; }
        DeviceStatus Status { get; set; }
        string ToString();
        string GetDiagnostics();
        event EventHandler<StatusRegister> OnInterrupt;
    }
}