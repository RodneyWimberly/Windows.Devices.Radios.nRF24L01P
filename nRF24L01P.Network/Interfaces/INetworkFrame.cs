namespace Windows.Devices.Radios.nRF24L01P.Network.Interfaces
{
    public interface INetworkFrame
    {
        INetworkHeader Header { get; set; }
        ushort MessageSize { get; set; }

        byte[] MessageBuffer { get; set; }
    }
}
