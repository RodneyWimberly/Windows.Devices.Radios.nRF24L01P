using System;
using System.Threading.Tasks;

namespace Windows.Devices.Radios.nRF24L01P
{
    public static class Utilities
    {
        public static void DelayMicroseconds(int microSeconds)
        {
            // We will assume that 10 ticks = 1 microsecond
            Task.Delay(new TimeSpan(10 * microSeconds)).Wait();
        }

        public static byte[] GetBytes(string hex)
        {
            if (hex.Length % 2 == 1)
                throw new ArgumentException("The hex string must have an even number of characters", nameof(hex));
            int length = hex.Length / 2;
            byte[] bytes = new byte[length];
            for (int index = 0; index < length; index++)
                bytes[index] = Convert.ToByte(Convert.ToInt32(hex.Substring(index * 2, 2), 16));
            return bytes;
        }

        public static string Byte2Hex(byte bits)
        {
            const string hex = "0123456789ABCDEF";
            int high = bits >> 4;
            int low = bits & 0x0F;
            return "0x" + new string(new char[] { hex[high], hex[low] });
        }
    }
}
