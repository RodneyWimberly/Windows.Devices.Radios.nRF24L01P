using System;
using System.Threading.Tasks;

namespace Windows.Devices.Radios.nRF24L01P
{
    public static class Utilities
    {
        public static byte BitValue(byte mask)
        {
            return (byte)(1 << mask);
        }

        public static void DelayMicroseconds(int microSeconds)
        {
            // We will assume that 10 ticks = 1 microsecond
            Task.Delay(new TimeSpan(10 * microSeconds)).Wait();
        }
    }
}
