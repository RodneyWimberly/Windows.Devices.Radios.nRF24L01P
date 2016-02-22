namespace Windows.Devices.Radios.nRF24L01P.Interfaces
{
    public interface ITransmitPipe : IDataPipe
    {
        void Write(byte[] data, bool disableAck = false);
    }
}