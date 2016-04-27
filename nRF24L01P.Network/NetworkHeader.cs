using Windows.Devices.Radios.nRF24L01P.Network.Enums;
using Windows.Devices.Radios.nRF24L01P.Network.Interfaces;

namespace Windows.Devices.Radios.nRF24L01P.Network
{
    public class NetworkHeader : INetworkHeader
    {
        public ushort FromNode { get; set; }

        public ushort Id { get; set; }

        public ushort NextId { get; set; }

        public byte Reserved { get; set; }

        public ushort ToNode { get; set; }

        public NetworkHeaderTypes Type { get; set; }

        public override string ToString()
        {
            return $"Id {Id} from {FromNode} to {ToNode} type {Type}";
        }
    }
}
