namespace Windows.Devices.Radios.nRF24L01P.Network.Interfaces
{
    public struct NodeAddressInfo : INodeAddressInfo
    {
        public ushort SendNode { get; set; }
        public byte SendPipe { get; set; }
        public bool MultiCast { get; set; }
    }
}