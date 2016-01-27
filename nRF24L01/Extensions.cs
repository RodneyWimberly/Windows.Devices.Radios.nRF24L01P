using Windows.Devices.Spi;

namespace Windows.Devices.Radios.nRF24L01
{
    public static class Extensions
    {
        public static byte Transfer(this SpiDevice spiDevice, byte request)
        {
            byte[] response = new byte[1];
            spiDevice.Write(new[] { request });
            spiDevice.Read(response);
            return response[0];
        }
    }
}
