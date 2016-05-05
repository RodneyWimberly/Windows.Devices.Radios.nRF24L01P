using System;
using Windows.Devices.Radios.nRF24L01P.Network.Interfaces;

namespace Windows.Devices.Radios.nRF24L01P.Network
{
    public class NetworkHeader : INetworkHeader
    {
        public ushort FromNode { get; set; }
        public ushort Id { get; set; }
        public byte Reserved { get; set; }
        public ushort ToNode { get; set; }
        public byte Type { get; set; }

        public NetworkHeader() { }

        public NetworkHeader(ushort toNode, byte type) : this()
        {
            ToNode = toNode;
            Type = type;
        }

        public NetworkHeader(byte[] bytes) : this()
        {
            FromBytes(bytes);
        }

        public byte[] ToBytes()
        {
            byte[] bytes = new byte[Configuration.NetworkHeaderSize],
                temp = BitConverter.GetBytes(FromNode);
            bytes[Configuration.HeaderIndexFromNode] = temp[0];
            bytes[Configuration.HeaderIndexFromNode + 1] = temp[1];

            temp = BitConverter.GetBytes(ToNode);
            bytes[Configuration.HeaderIndexToNode] = temp[0];
            bytes[Configuration.HeaderIndexToNode + 1] = temp[1];

            temp = BitConverter.GetBytes(Id);
            bytes[Configuration.HeaderIndexId] = temp[0];
            bytes[Configuration.HeaderIndexId + 1] = temp[1];

            bytes[Configuration.HeaderIndexType] = Type;
            bytes[Configuration.HeaderIndexReserved] = Reserved;

            return bytes;
        }

        public void FromBytes(byte[] bytes)
        {
            if (bytes.Length < Configuration.NetworkHeaderSize)
                throw new ArgumentException(string.Format("Array is the incorrect size. Expected {0} but it was actually {1}", Configuration.NetworkHeaderSize, bytes.Length), nameof(bytes));

            FromNode = BitConverter.ToUInt16(bytes, Configuration.HeaderIndexFromNode);
            ToNode = BitConverter.ToUInt16(bytes, Configuration.HeaderIndexToNode);
            Id = BitConverter.ToUInt16(bytes, Configuration.HeaderIndexId);
            Type = bytes[Configuration.HeaderIndexType];
            Reserved = bytes[Configuration.HeaderIndexReserved];
        }

        public override string ToString()
        {
            return $"Id {Id} from {FromNode} to {ToNode} type {Type}";
        }

        public static implicit operator byte[] (NetworkHeader networkHeader)
        {
            return networkHeader.ToBytes();
        }

        public static implicit operator NetworkHeader(byte[] bytes)
        {
            return new NetworkHeader(bytes);
        }
    }
}
