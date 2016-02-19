using Newtonsoft.Json;
using System;
using Windows.Devices.Radios.nRF24L01P.Enums;
using Windows.Devices.Radios.nRF24L01P.Interfaces;

namespace Windows.Devices.Radios.nRF24L01P.Registers
{
    public abstract class RegisterBase : IRegisterBase
    {
        protected ICommandProcessor CommandProcessor;

        public byte[] Value { get; private set; }
        public bool IsDirty { get; private set; }
        public int Length { get; }
        public byte Address { get; }
        public string Name { get; }

        protected RegisterBase(ICommandProcessor commandProcessor, int length, byte address, string name = "")
        {
            Value = new byte[length];
            Name = string.IsNullOrEmpty(name) ? GetType().Name : name;
            CommandProcessor = commandProcessor;
            Length = length;
            Address = address;
            IsDirty = false;
        }

        public void Load()
        {
            Load(CommandProcessor.ExecuteCommand(DeviceCommands.R_REGISTER, Address, Value));
        }

        public void Load(byte[] value)
        {
            Value = value;
            IsDirty = false;
        }

        public void Save()
        {
            CommandProcessor.ExecuteCommand(DeviceCommands.W_REGISTER, Address, Value);
            IsDirty = false;
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

        protected bool GetBoolProperty(BitMasks propertyMask)
        {
            return (Value[0] & (byte)propertyMask) > 0;
            //return GetByteProperty(propertyMask) > 0;
        }

        protected byte GetByteProperty(BitMasks propertyMask)
        {
            return (byte)(Value[0] & (byte)propertyMask);
        }

        protected void SetBoolProperty(BitMasks propertyMask, bool value)
        {
            Value[0] = (byte)(value ? (Value[0] | (byte)propertyMask) : (Value[0] & ~(byte)propertyMask));
            IsDirty = true;
        }

        protected void SetByteProperty(BitMasks propertyMask, byte value)
        {
            Value[0] = (byte)((value & (byte)propertyMask) | (Value[0] & ~(byte)propertyMask));
            IsDirty = true;
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}
