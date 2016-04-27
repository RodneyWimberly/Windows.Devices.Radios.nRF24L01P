using System.Diagnostics;
using Windows.Devices.Radios.nRF24L01P.Common.Extensions;

namespace Windows.Devices.Radios.nRF24L01P.Common
{
    public static class Delay
    {
        private static readonly object SyncRoot;

        static Delay()
        {
            SyncRoot = new object();
        }

        public static void WaitMilliseconds(int milliseconds)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            lock (SyncRoot)
            {
                while (true)
                {
                    if (stopwatch.ElapsedMilliseconds() >= milliseconds) break;
                }
            }
        }

        public static void WaitMicroseconds(int microseconds)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            lock (SyncRoot)
            {
                while (true)
                {
                    if (stopwatch.ElapsedMicroseconds() >= microseconds) break;
                }
            }
        }
    }
}
