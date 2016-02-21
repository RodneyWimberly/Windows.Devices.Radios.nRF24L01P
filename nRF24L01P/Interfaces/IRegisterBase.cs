namespace Windows.Devices.Radios.nRF24L01P.Interfaces
{
    public interface IRegisterBase
    {
        byte[] Value { get; }
        string HexValue { get; }
        int Length { get; }
        byte Address { get; }
        string Name { get; }
        bool IsDirty { get; }
        void Load();
        void Load(byte[] value);
        void Save();
        string ToString();
    }
}