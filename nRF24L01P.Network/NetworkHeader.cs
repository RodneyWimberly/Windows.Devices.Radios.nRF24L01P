using System;
using Windows.Devices.Radios.nRF24L01P.Network.Interfaces;

namespace Windows.Devices.Radios.nRF24L01P.Network
{
    public class NetworkHeader : INetworkHeader
    {
        public ushort FromNode { get; set; }

        public ushort Id { get; set; }

        public ushort NextId { get; set; }

        public byte Reserved { get; set; }

        public ushort ToNode { get; set; }

        public byte Type { get; set; }

        public NetworkHeader() { }

        public NetworkHeader(byte[] bytes) : this()
        {
            FromBytes(bytes);
        }

        public byte[] ToBytes()
        {
            return new byte[Configuration.FrameHeaderSize];
        }

        public void FromBytes(byte[] bytes)
        {
            if (bytes.Length < Configuration.FrameHeaderSize)
                throw new ArgumentException(string.Format("Array is the incorrect size. Expected {0} but it was actually {1}", Configuration.FrameHeaderSize, bytes.Length), nameof(bytes));

            byte[] headerBytes = new byte[Configuration.FrameHeaderSize];
            Array.Copy(bytes, headerBytes, Configuration.FrameHeaderSize);
        }

        public static implicit operator byte[] (NetworkHeader networkHeader)
        {
            return networkHeader.ToBytes();
        }

        public static implicit operator NetworkHeader(byte[] bytes)
        {
            NetworkHeader header = new NetworkHeader();
            header.FromBytes(bytes);
            return header;
        }

        public override string ToString()
        {
            return $"Id {Id} from {FromNode} to {ToNode} type {Type}";
        }
    }
}
