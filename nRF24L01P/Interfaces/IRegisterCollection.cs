using System.Collections.Generic;

namespace Windows.Devices.Radios.nRF24L01P.Interfaces
{
    public interface IRegisterCollection<T> : IDictionary<byte, T> where T : IRegisterBase
    {
        void LoadAll();
        void SaveAll(bool force = false);
        string ToString();
    }
}