namespace Windows.Devices.Radios.nRF24L01P.Network.Interfaces
{
    public interface INetworkHeader
    {
        ushort FromNode { get; set; }
        ushort ToNode { get; set; }
        ushort Id { get; set; }
        byte Type { get; set; }
        byte Reserved { get; set; }
        ushort NextId { get; set; }
        byte[] ToBytes();
        void FromBytes(byte[] bytes);
    }
}
