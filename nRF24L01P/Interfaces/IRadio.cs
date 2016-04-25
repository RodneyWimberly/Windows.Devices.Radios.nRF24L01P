using System;
using Windows.Devices.Radios.nRF24L01P.Enums;

namespace Windows.Devices.Radios.nRF24L01P.Interfaces
{
    public interface IRadio
    {
        IRegisterContainer RegisterContainer { get; }
        IConfiguration Configuration { get; }
        ITransmitPipe TransmitPipe { get; }
        IReceivePipeCollection ReceivePipes { get; }
        OperatingModes OperatingMode { get; set; }
        string ToString();
        string GetArduinoDetails();

        event EventHandler<InterruptedEventArgs> Interrupted;
    }
}