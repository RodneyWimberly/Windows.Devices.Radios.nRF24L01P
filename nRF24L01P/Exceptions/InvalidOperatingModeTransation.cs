using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Devices.Radios.nRF24L01P.Enums;
using Windows.Devices.Radios.nRF24L01P.Extensions;

namespace Windows.Devices.Radios.nRF24L01P.Exceptions
{
    public class InvalidOperatingModeTransitionException : Exception
    {
        public InvalidOperatingModeTransitionException(OperatingModes oldStatus, OperatingModes newStatus, OperatingModes[] validModes) :
           base(string.Format("Invalid status transition from {0} to {1}, you can only transition into {1} mode from {2} mode.",
               oldStatus,
               newStatus,
               ValidModes(validModes)))
        {
        }

        private static string ValidModes(IEnumerable<OperatingModes> validModes)
        {
            string modes = validModes.Aggregate(string.Empty, (list, mode) => list + mode.GetName() + ", ");
            return modes.Substring(0, modes.Length - 2);
        }
    }
}
