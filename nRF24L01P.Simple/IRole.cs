using Windows.Devices.Radios.nRF24L01P.Interfaces;

namespace Windows.Devices.Radios.nRF24L01P.Simple
{
    public interface IRole
    {
        void AttachRadio(IRadio radio);
        void DetachRadio();
        bool Start();
        void Stop();
    }
}
