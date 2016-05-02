using Common.Logging;
using System;
using System.Linq;
using Windows.Devices.Radios.nRF24L01P.Common;
using Windows.Devices.Radios.nRF24L01P.Common.Enums;
using Windows.Devices.Radios.nRF24L01P.Common.Extensions;
using Windows.Devices.Radios.nRF24L01P.Enums;
using Windows.Devices.Radios.nRF24L01P.Interfaces;

namespace Windows.Devices.Radios.nRF24L01P.Registers
{
    public abstract class RegisterBase : IRegister
    {
        protected ILog Logger;
        protected ICommandProcessor CommandProcessor;
        protected byte[] Buffer;
        protected readonly byte[] DefaultValue;
        private readonly object _syncRoot;

        public string Value => Buffer.GetHexString("0x");

        public bool IsDirty { get; protected set; }
        public int Length { get; protected set; }
        public byte Address { get; protected set; }
        public string Name { get; protected set; }

        protected RegisterBase(ILoggerFactoryAdapter loggerFactoryAdapter, ICommandProcessor commandProcessor, int length, byte address, byte[] defaultValue, string name = "")
        {
            Logger = loggerFactoryAdapter.GetLogger(GetType());
            _syncRoot = new object();
            Buffer = new byte[length];
            Name = GetType().Name + (string.IsNullOrEmpty(name) ? "" : string.Format(" ({0})", name));
            CommandProcessor = commandProcessor;
            Length = length;
            Address = address;
            DefaultValue = defaultValue;
            IsDirty = false;
        }

        public void Load()
        {
            lock (_syncRoot)
            {
                Load(CommandProcessor.ExecuteCommand(DeviceCommands.R_REGISTER, Address, Buffer));
                // We called Load after reading from Register so we know the value is not dirty
                IsDirty = false;
            }
        }

        public void Load(byte[] value)
        {
            Buffer = value;
            // Value was set externally so we need to mark the buffer as dirty.
            IsDirty = true;
        }

        public void Save(bool force = false)
        {
            if (!IsDirty && !force) return;
            lock (_syncRoot)
            {
                CommandProcessor.ExecuteCommand(DeviceCommands.W_REGISTER, Address, Buffer);
                IsDirty = false;
            }
        }

        public void ResetToDefault()
        {
            lock (_syncRoot)
            {
                Buffer = DefaultValue;
                CommandProcessor.ExecuteCommand(DeviceCommands.W_REGISTER, Address, DefaultValue);
            }
        }

        public static implicit operator byte[] (RegisterBase source)
        {
            return source.Buffer;
        }

        public static implicit operator byte(RegisterBase source)
        {
            if (source.Length == 1)
                return source.Buffer[0];
            throw new InvalidOperationException("Cannot convert register value to single byte while register length is greater than 1");
        }

        protected bool GetBoolProperty(BitMasks propertyMask)
        {
            return GetByteProperty(propertyMask) > 0;
        }

        protected byte GetByteProperty(BitMasks propertyMask)
        {
            return (byte)((Buffer[0] & (byte)propertyMask) >> GetShiftLevel(propertyMask));
        }

        protected void SetBoolProperty(BitMasks propertyMask, bool value)
        {
            byte mask = (byte)propertyMask;
            Buffer[0] = (byte)(value ? (Buffer[0] | mask) : (Buffer[0] & ~mask));
            IsDirty = true;
        }

        protected void SetByteProperty(BitMasks propertyMask, byte value)
        {
            byte mask = (byte)propertyMask;
            value = (byte)(value << GetShiftLevel(propertyMask));
            Buffer[0] = (byte)((value & mask) | (Buffer[0] & ~mask));
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
            return JsonHelper.ToJson(this);
        }
    }
}
