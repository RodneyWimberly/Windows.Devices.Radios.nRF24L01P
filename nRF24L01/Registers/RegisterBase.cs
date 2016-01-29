using Newtonsoft.Json;
using System;

namespace Windows.Devices.Radios.nRF24L01.Registers
{
    public abstract class RegisterBase
    {
        private byte[] _valueBuffer;

        public byte[] Value
        {
            get { return _valueBuffer; }
            internal set { _valueBuffer = value; }
        }

        public byte FirstByte
        {
            get { return _valueBuffer[0]; }
            internal set { _valueBuffer[0] = value; }
        }

        public int Length { get; private set; }
        public byte Address { get; private set; }

        public string Name { get; private set; }

        protected Radio Radio;

        protected RegisterBase(Radio radio, int length, byte address, string name = "")
        {
            Name = string.IsNullOrEmpty(name) ? GetType().Name : name;
            Radio = radio;
            Length = length;
            Value = new byte[length];
            Address = address;
        }

        public void Load()
        {
            byte[] request = new byte[Length + 1];
            request[0] = (byte)(Constants.R_REGISTER | Address);
            Array.Copy(Value, 0, request, 1, Length);
            Value = Radio.Transfer(request);
        }

        public void Save()
        {
            byte[] buffer = new byte[Length + 1];
            buffer[0] = (byte)(Constants.W_REGISTER | Address);
            Array.Copy(Value, 0, buffer, 1, Length);
            Radio.Transfer(buffer);

            Load();
        }

        public static implicit operator byte[] (RegisterBase source)
        {
            return source.Value;
        }

        public static implicit operator byte(RegisterBase source)
        {
            if (source.Length == 1)
                return source.Value[0];
            throw new InvalidOperationException("cannot convert register value to single byte while register length is greater than 1");
        }

        protected bool GetBitValue(byte mask)
        {
            return (FirstByte & mask) > 0;
        }

        protected byte SetBitValue(byte mask, bool value)
        {
            return (byte)(value ? (FirstByte | mask) : (FirstByte & ~mask));
        }

        /// <summary>
        /// extract the bits from byte
        /// example: 
        /// Value=1110 0000
        /// start=7
        /// end =5
        /// Output=0000 0111
        /// </summary>
        /// <param name="value"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        protected byte GetByteValue(int start, int end)
        {
            return (byte)(FirstByte << (7 - start) >> (7 - (start - end)));
        }

        /// <summary>
        /// fill the target byte with spefied bits from source byte
        /// example:
        /// target = 1111 1111
        /// source = 0000 0000
        /// start = 6
        /// end = 3
        /// Output= 1100 0011
        /// </summary>
        /// <param name="target"></param>
        /// <param name="source"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        protected byte SetByteValue(byte source, int start, int end)
        {
            if (start < end)
                throw new ArgumentOutOfRangeException("end", "end value cannot be larget than start value");
            byte mask = CreateBitMask(start, end);
            int masked_source = (source & mask); //extract the source bits
            int masked_target = (FirstByte & ~mask); //clear the bits at target byte
            return (byte)(masked_source | masked_target);
        }

        protected byte CreateBitMask(int start, int end)
        {
            int result = 0;
            for (int i = start; i >= end; i--)
                result = result | 0X01 << i;
            return (byte)result;
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}
