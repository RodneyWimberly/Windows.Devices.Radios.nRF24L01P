namespace Windows.Devices.Radios.nRF24L01P.Network.Interfaces
{
    public interface INetworkAddressing
    {
        ulong PipeAddress(ushort node, byte pipeNo);
        ushort NodeAddress { get; set; }
        ushort ParentNode { get; set; }
        byte ParentPipe { get; set; }
        ushort NodeMask { get; set; }

        ushort Parent();
        ushort AddressOfPipe(ushort node, byte pipeNo);
        bool IsValidAddress(ushort node);

        bool IsDirectChild(ushort node);
        bool IsDescendant(ushort node);
        ushort DirectChildRouteTo(ushort node);
        void SetupAddress();
        INodeAddressInfo ConvertLogicalToPhysicalAddress(INodeAddressInfo conversionInfo);
        ushort LevelToAddress(byte level);
    }
}
