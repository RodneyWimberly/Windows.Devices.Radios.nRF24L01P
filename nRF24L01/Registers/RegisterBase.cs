using Newtonsoft.Json;
using System;

namespace Windows.Devices.Radios.nRF24L01P.Registers
{
    public abstract class RegisterBase
    {
        protected Radio Radio;

        public byte[] Value { get; private set; }

        public int Length { get; private set; }
        public byte Address { get; private set; }
        public string Name { get; private set; }

        protected RegisterBase(Radio radio, int length, byte address, string name = "")
        {
            Value = new byte[length];
            Name = string.IsNullOrEmpty(name) ? GetType().Name : name;
            Radio = radio;
            Length = length;
            Address = address;
        }

        public void Load()
        {
            byte[] request = new byte[Length + 1];
            request[0] = (byte)(Commands.R_REGISTER | (Commands.REGISTER_MASK & Address));
            Array.Copy(Value, 0, request, 1, Length);
            Load(Radio.Transfer(request));
        }

        public void Load(byte[] value)
        {
            Value = value;
        }

        public void Save()
        {
            byte[] buffer = new byte[Length + 1];
            buffer[0] = (byte)(Commands.W_REGISTER | (Commands.REGISTER_MASK & Address));
            Array.Copy(Value, 0, buffer, 1, Length);
            Radio.Transfer(buffer);

            //Load();
        }

        public static implicit operator byte[] (RegisterBase source)
        {
            return source.Value;
        }

        public static implicit operator byte(RegisterBase source)
        {
            if (source.Length == 1)
                return source.Value[0];
            throw new InvalidOperationException("Cannot convert register value to single byte while register length is greater than 1");
        }

        protected bool GetBitValue(byte mask)
        {
            return (Value[0] & mask) > 0;
        }

        protected void SetBitValue(byte mask, bool value)
        {
            Value[0] = (byte)(value ? (Value[0] | mask) : (Value[0] & ~mask));
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
            return (byte)(Value[0] << (7 - start) >> (7 - (start - end)));
        }

        /// <summary>
        /// Fill the target byte with specified bits from source byte
        /// Example:
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
        protected void SetByteValue(byte source, int start, int end)
        {
            if (start < end)
                throw new ArgumentOutOfRangeException("end", "end value cannot be larget than start value");
            byte mask = CreateBitMask(start, end);
            int maskedSource = (source & mask); //extract the source bits
            int maskedTarget = (Value[0] & ~mask); //clear the bits at target byte
            Value[0] = (byte)(maskedSource | maskedTarget);
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
