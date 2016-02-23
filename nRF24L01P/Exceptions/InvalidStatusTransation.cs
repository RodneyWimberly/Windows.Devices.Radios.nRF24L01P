using System;
using Windows.Devices.Radios.nRF24L01P.Enums;

namespace Windows.Devices.Radios.nRF24L01P.Exceptions
{
    public class InvalidStatusTransitionException : Exception
    {
        public InvalidStatusTransitionException(DeviceStatus oldStatus, DeviceStatus newStatus, string message) :
            base(string.Format("Invalid status transition from {0} to {1}, {2}", oldStatus, newStatus, message))
        {
        }
    }
}
