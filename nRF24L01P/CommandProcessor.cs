using System;
using Windows.Devices.Radios.nRF24L01P.Enums;
using Windows.Devices.Spi;

namespace Windows.Devices.Radios.nRF24L01P
{
    public class CommandProcessor : ICommandProcessor
    {
        private readonly bool _revertBytes;
        private readonly bool _checkStatus;
        protected readonly object SyncRoot;
        private readonly SpiDevice _spiDevice;
        private readonly Radio _radio;
        public Action<byte[]> StatusRegisterLoad { get; set; }

        public CommandProcessor(SpiDevice spiDevice,Radio radio, bool checkStatus = true)
        {
            //chip uses LSByte first, we should revert byte array order on MSByte first CPU
            _revertBytes = BitConverter.IsLittleEndian;
            _checkStatus = checkStatus;
            SyncRoot = new object();
            _spiDevice = spiDevice;
            _radio = radio;
        }

        /// <summary>
        /// write command with address to SPI
        /// </summary>
        /// <param name="deviceCommand"></param>
        /// <param name="address"></param>
        /// <param name="value">32 bytes max, no exception handling here</param>
        /// <returns></returns>
        public byte[] ExecuteCommand(DeviceCommands deviceCommand, byte address, byte[] value, bool autoRevert = true)
        {
            CheckStatus(deviceCommand);

            // Create send and receive buffers
            int resultLength = value.Length,
                bufferLength = resultLength + 1;
            byte[] result = new byte[resultLength];
            byte[] sendBuffer = new byte[bufferLength],
                receiveBuffer = new byte[bufferLength];

            // Populate send buffer with command and value to write
            sendBuffer[0] = (byte)((byte)deviceCommand | address);
            if (value.Length > 1 && autoRevert && _revertBytes)
                for (int i = 0; i < resultLength; i++)
                    sendBuffer[resultLength - i] = value[i];
            else
                Array.Copy(value, 0, sendBuffer, 1, resultLength);

            // Send and Receive
            lock (SyncRoot)
            {
                _spiDevice.TransferFullDuplex(sendBuffer, receiveBuffer);
            }

            // The STATUS register value is returned at first byte on each SPI call
            StatusRegisterLoad(new[] {receiveBuffer[0]});

            // Populate result with the rest of the receive buffer
            if (value.Length > 1 && autoRevert && _revertBytes) 
                for (int i = 0; i < result.Length; i++)
                    result[i] = receiveBuffer[bufferLength - (i + 1)];
            else
                Array.Copy(receiveBuffer, 1, result, 0, bufferLength - 1);

            return result;
        }

        /// <summary>
        /// write single command to SPI, return value is status register value
        /// </summary>
        /// <param name="deviceCommand"></param>
        /// <param name="address"></param>
        public void ExecuteCommand(DeviceCommands deviceCommand, byte address)
        {
            CheckStatus(deviceCommand);

            byte[] sendBuffer = new byte[1],
                    receiveBuffer = new byte[1];
            sendBuffer[0] = (byte)((byte)deviceCommand | address); 
            lock (SyncRoot)
            {
                _spiDevice.TransferFullDuplex(sendBuffer, receiveBuffer);
            }
            StatusRegisterLoad(receiveBuffer);
        }

        /// <summary>
        /// write single command to SPI, return value is status register value
        /// </summary>
        /// <param name="deviceCommand"></param>
        public void ExecuteCommand(DeviceCommands deviceCommand)
        {
            CheckStatus(deviceCommand);

            byte[] sendBuffer = new byte[1],
                    receiveBuffer = new byte[1];
            sendBuffer[0] = (byte)deviceCommand;
            lock (SyncRoot)
            {
                _spiDevice.TransferFullDuplex(sendBuffer, receiveBuffer);
            }
            StatusRegisterLoad(receiveBuffer);
        }

        private void CheckStatus(DeviceCommands deviceCommand)
        {
            // Can we write at this time
            if (_checkStatus && (deviceCommand == DeviceCommands.W_REGISTER && !(_radio.Status == DeviceStatus.StandBy || _radio.Status == DeviceStatus.PowerDown)))
                throw new InvalidOperationException("Writing register should only in Standby or PowerDown mode");
        }
    }
}
