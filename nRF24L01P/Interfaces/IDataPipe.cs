using Windows.Devices.Radios.nRF24L01P.Enums;

namespace Windows.Devices.Radios.nRF24L01P.Interfaces
{
    public interface IDataPipe
    {
        byte[] Address { get; set; }
        FifoStatus FifoStatus { get; }
        void FlushBuffer();
    }
}
