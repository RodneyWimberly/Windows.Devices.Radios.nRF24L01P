using System.Linq;

namespace Windows.Devices.Radios.nRF24L01P.Common.Extensions
{
    public static class ByteExtensions
    {
        public static byte[] ReverseBytes(this byte[] bytes)
        {
            int length = bytes.Length;
            byte[] reverseBytes = new byte[length];
            
            for (int i = 0; i < length; i++)
                reverseBytes[(length -1) - i] = bytes[i];
            return reverseBytes;
        }

        public static string GetHexString(this byte[] bytes)
        {
            return bytes.GetHexString(string.Empty);
        }

        public static string GetHexString(this byte[] bytes, string preFix)
        {
            return bytes.Aggregate(preFix, (current, part) => current + part.ToString("X").PadLeft(2, '0'));
        }
    }
}
