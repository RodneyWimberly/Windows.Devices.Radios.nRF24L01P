namespace Windows.Devices.Radios.nRF24L01P.Interfaces
{
    public interface IRole
    {
        void AttachDevice(Radio radio);
        void DetachDevice();
        void Start();
        void Stop();
    }
}
