using System;
using Windows.Devices.Radios.nRF24L01P.Enums;

namespace Windows.Devices.Radios.nRF24L01P
{
    public interface ICommandProcessor
    {
        Action<byte[]> StatusRegisterLoad { get; set; }

        /// <summary>
        /// write command with address to SPI
        /// </summary>
        /// <param name="command"></param>
        /// <param name="address"></param>
        /// <param name="value">32 bytes max, no exception handling here</param>
        /// <returns></returns>
        byte[] ExecuteCommand(DeviceCommands deviceCommand, byte address, byte[] value, bool autoRevert = true);

        /// <summary>
        /// write single command to SPI, return value is status register value
        /// </summary>
        /// <param name="command"></param>
        /// <param name="address"></param>
        void ExecuteCommand(DeviceCommands deviceCommand, byte address);

        /// <summary>
        /// write single command to SPI, return value is status register value
        /// </summary>
        /// <param name="command"></param>
        void ExecuteCommand(DeviceCommands deviceCommand);
    }
}