using System;
using Windows.Devices.Radios.nRF24L01P.Enums;

namespace Windows.Devices.Radios.nRF24L01P.Interfaces
{
    public interface ICommandProcessor
    {
        bool CheckStatus { get; set; }

        Action<byte[]> LoadStatusRegister { get; set; }

        /// <summary>
        /// write command with address to SPI
        /// </summary>
        /// <param name="deviceCommand"></param>
        /// <param name="address"></param>
        /// <param name="value">32 bytes max, no exception handling here</param>
        /// <returns></returns>
        byte[] ExecuteCommand(DeviceCommands deviceCommand, byte address, byte[] value, bool autoRevert = true);

        /// <summary>
        /// write single command to SPI, return value is status register value
        /// </summary>
        /// <param name="deviceCommand"></param>
        /// <param name="address"></param>
        byte ExecuteCommand(DeviceCommands deviceCommand, byte address);

        /// <summary>
        /// write single command to SPI, return value is status register value
        /// </summary>
        /// <param name="deviceCommand"></param>
        byte ExecuteCommand(DeviceCommands deviceCommand);
    }
}