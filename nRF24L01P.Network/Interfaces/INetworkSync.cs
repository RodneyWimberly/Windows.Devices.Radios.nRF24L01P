namespace Windows.Devices.Radios.nRF24L01P.Network.Interfaces
{
    public interface INetworkSync
    {
        void Begin(ushort toNode);
        void RegisterMe<T>(T data);
        void Reset();
        void Update();
    }
}
