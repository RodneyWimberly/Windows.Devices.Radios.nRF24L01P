namespace Windows.Devices.Radios.nRF24L01
{
    public interface IRadio
    {
        RadioKind RadioKind { get; }
        RadioState RadioState { get; set; }
        byte PayloadSize { get; set; }
        bool IsAckPayloadAvailable { get; }
        DataRates DataRate { get; set; }
        CrcLengths CrcLength { get; set; }
        PowerLevels PowerLevel { get; set; }
        byte Channel { get; set; }
        RadioModels RadioModel { get; }
        string Name { get; }
        bool IsDataAvailable { get; }
        void Begin();
        void StartListening();
        void StopListening();
        bool Write(byte[] data);
        void StartWrite(byte[] data);
        byte GetDynamicPayloadSize();
        bool Available();
        bool Available(byte[] pipes);
        bool Read(byte[] readBuffer, int length);
        void WhatHappened(out bool txOk, out bool txFail, out bool rxReady);
        void OpenWritingPipe(long value);
        void OpenReadingPipe(byte child, long address);
        void EnableDynamicPayloads();
        void EnableAckPayload();
        void WriteAckPayload(byte pipe, byte[] data, int length);
        void ToggleFeatures();
        void PowerDown();
        void PowerUp();
        bool TestCarrier();
        bool TestRpd();
        string GetDetails();
        void SetAutoAck(bool enable);
        void SetAutoAck(byte pipe, bool enable);
        void SetRetries(byte delay, byte count);
    }
}