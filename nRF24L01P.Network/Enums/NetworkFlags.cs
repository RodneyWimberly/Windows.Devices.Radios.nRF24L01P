using System;

namespace Windows.Devices.Radios.nRF24L01P.Network.Enums
{
    [Flags]
    public enum NetworkFlags
    {
        HoldIncoming = 1,
        BypassHolds = 2,
        FastFrag = 4,
        NoPoll = 8
    }
}
