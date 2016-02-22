namespace Windows.Devices.Radios.nRF24L01P.Interfaces
{
    public interface IRegisterBase
    {
        string Value { get; }
        int Length { get; }
        byte Address { get; }
        string Name { get; }
        bool IsDirty { get; }
        void Load();
        void Load(byte[] value);
        void Save(bool force = false);
        void Reset();
        string ToString();
    }
}