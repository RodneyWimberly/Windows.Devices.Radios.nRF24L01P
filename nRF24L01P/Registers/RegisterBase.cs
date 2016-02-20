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
            if (!IsDirty) return;
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
            return GetByteProperty(propertyMask) > 0;
        }

        protected byte GetByteProperty(BitMasks propertyMask)
        {
            return (byte)((Value[0] & (byte)propertyMask) >> GetShiftLevel(propertyMask));
        }

        protected void SetBoolProperty(BitMasks propertyMask, bool value)
        {
            byte mask = (byte)propertyMask;
            Value[0] = (byte)(value ? (Value[0] | mask) : (Value[0] & ~mask));
            IsDirty = true;
        }

        protected void SetByteProperty(BitMasks propertyMask, byte value)
        {
            value = (byte)(value << GetShiftLevel(propertyMask));
            byte mask = (byte)propertyMask;
            Value[0] = (byte)((value & mask) | (Value[0] & ~mask));
            IsDirty = true;
        }

        private int GetShiftLevel(BitMasks propertyMask)
        {
            int shiftLevel = 0;
            if (propertyMask.HasFlag(BitMasks.Bit0))
                shiftLevel = 0;
            else if (propertyMask.HasFlag(BitMasks.Bit1))
                shiftLevel = 1;
            else if (propertyMask.HasFlag(BitMasks.Bit2))
                shiftLevel = 2;
            else if (propertyMask.HasFlag(BitMasks.Bit3))
                shiftLevel = 3;
            else if (propertyMask.HasFlag(BitMasks.Bit4))
                shiftLevel = 4;
            else if (propertyMask.HasFlag(BitMasks.Bit5))
                shiftLevel = 5;
            else if (propertyMask.HasFlag(BitMasks.Bit6))
                shiftLevel = 6;
            else if (propertyMask.HasFlag(BitMasks.Bit7))
                shiftLevel = 7;
            return shiftLevel;
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}
