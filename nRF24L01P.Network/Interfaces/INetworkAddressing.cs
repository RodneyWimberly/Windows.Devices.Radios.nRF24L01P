namespace Windows.Devices.Radios.nRF24L01P.Network.Interfaces
{
    public interface INetworkAddressing
    {
        ushort NodeAddress { get; set; }
        ushort ParentNodeAddress { get; set; }
        byte ParentPipe { get; set; }
        ushort NodeMask { get; set; }
        ushort LogicalPipeAddress(ushort node, byte pipeNo);
        byte[] PhysicalPipeAddress(ushort node, byte pipeNo);
        bool IsValidAddress(ushort node);
        bool IsDirectChild(ushort node);
        bool IsDescendant(ushort node);
        ushort DirectChildRouteTo(ushort node);
        INodeAddressInfo ConvertLogicalToPhysicalAddress(INodeAddressInfo conversionInfo);
        ushort LevelToAddress(byte level);
    }
}
