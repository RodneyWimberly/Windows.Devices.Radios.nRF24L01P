using System;
using Windows.Devices.Radios.nRF24L01P.Enums;
using Windows.Devices.Radios.nRF24L01P.Interfaces;

namespace nRF24L01P.UnitTests
{
    public class MockCommandProcessor : ICommandProcessor
    {
        public bool CheckStatusField;
        public bool CheckOperatingMode
        {
            get
            {
                return CheckStatusField;
            }

            set
            {
                CheckStatusField = value;
            }
        }

        public Func<OperatingModes> GetDeviceStatusField;
        public Func<OperatingModes> GetOperatingMode
        {
            get
            {
                return GetDeviceStatusField;
            }

            set
            {
                GetDeviceStatusField = value;
            }
        }

        public Action<byte[]> LoadStatusRegisterField;
        public Action<byte[]> LoadStatusRegister
        {
            get
            {
                return LoadStatusRegisterField;
            }

            set
            {
                LoadStatusRegisterField = value;
            }
        }

        public int ChipSelectLine { get; set; }

        public string ControllerName { get; set; }

        public byte[] ExecuteCommandReturns;
        public byte ExecuteCommand(DeviceCommands deviceCommand)
        {
            return ExecuteCommandReturns[0];
        }

        public byte ExecuteCommand(DeviceCommands deviceCommand, byte address)
        {
            return ExecuteCommandReturns[0];
        }

        public byte[] ExecuteCommand(DeviceCommands deviceCommand, byte address, byte[] value, bool autoRevert = true)
        {
            return ExecuteCommandReturns;
        }

        public void Dispose()
        {
        }
    }
}
