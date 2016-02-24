namespace Windows.Devices.Radios.nRF24L01P.Extensions
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
    }
}
