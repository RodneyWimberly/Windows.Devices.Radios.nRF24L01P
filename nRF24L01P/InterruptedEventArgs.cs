using System;
using Windows.Devices.Radios.nRF24L01P.Registers;

namespace Windows.Devices.Radios.nRF24L01P
{
    public class InterruptedEventArgs : EventArgs
    {
        public StatusRegister StatusRegister { get; set; }
    }
}
