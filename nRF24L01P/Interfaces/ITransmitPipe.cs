using Windows.Devices.Radios.nRF24L01P.Enums;

namespace Windows.Devices.Radios.nRF24L01P.Interfaces
{
    public interface ITransmitPipe : IDataPipe
    {
        void Write(byte[] data, bool disableACK = false);
    }
}