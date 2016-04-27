namespace Windows.Devices.Radios.nRF24L01P.Network.Interfaces
{
    public interface INodeAddressInfo
    {
        ushort SendNode { get; set; }
        byte SendPipe { get; set; }
        bool MultiCast { get; set; }
    }
}