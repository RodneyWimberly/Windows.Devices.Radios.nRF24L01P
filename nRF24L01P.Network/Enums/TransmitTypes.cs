
namespace Windows.Devices.Radios.nRF24L01P.Network.Enums
{
    public enum TransmitTypes : byte
    {
        TransmitNormal = 0,
        TransmitRouted = 1,
        UserTransmitToPhysicalAddress = 2, //no network ACK
        UserTransmitToLogicalAddress = 3, // network ACK
        UserTransmitMulticast = 4
    }
}
