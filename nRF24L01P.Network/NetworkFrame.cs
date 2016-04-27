using Windows.Devices.Radios.nRF24L01P.Network.Interfaces;

namespace Windows.Devices.Radios.nRF24L01P.Network
{
    public class NetworkFrame : INetworkFrame
    {
        public INetworkHeader Header { get; set; }

        public byte[] MessageBuffer { get; set; }

        public ushort MessageSize { get; set; }
    }
}
