using System;
using System.Diagnostics;

namespace Windows.Devices.Radios.nRF24L01P.Common.Extensions
{
    public static class StopWatchExtensions
    {
        public static double ElapsedMilliseconds(this Stopwatch stopwatch)
        {
            if (stopwatch == null)
                throw new ArgumentNullException(nameof(stopwatch), "Stopwatch passed cannot be null!");

            return 1000 * stopwatch.ElapsedTicks / (double)Stopwatch.Frequency;
        }

        public static double ElapsedMicroseconds(this Stopwatch stopwatch)
        {
            if (stopwatch == null)
                throw new ArgumentNullException(nameof(stopwatch), "Stopwatch passed cannot be null!");

            return 1e6 * stopwatch.ElapsedTicks / (double)Stopwatch.Frequency;
        }
    }
}
