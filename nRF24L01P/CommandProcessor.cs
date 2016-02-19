using System;
using System.Threading.Tasks;
using Windows.Devices.Radios.nRF24L01P.Enums;
using Windows.Devices.Radios.nRF24L01P.Interfaces;
using Windows.Devices.Spi;

namespace Windows.Devices.Radios.nRF24L01P
{
    public class CommandProcessor : ICommandProcessor
    {
        private readonly bool _revertBytes;
        private readonly SpiDevice _spiDevice;

        protected readonly object SyncRoot;

        public Action<byte[]> LoadStatusRegister { get; set; }
        public Func<DeviceStatus> GetDeviceStatus { get; set; }
        public bool CheckStatus { get; set; }

        public CommandProcessor(SpiDevice spiDevice, bool checkStatus = true)
        {
            _revertBytes = BitConverter.IsLittleEndian;
            CheckStatus = checkStatus;
            SyncRoot = new object();
            _spiDevice = spiDevice;
        }

        public byte[] ExecuteCommand(DeviceCommands deviceCommand, byte address, byte[] value, bool autoRevert = true)
        {
            CanExecuteCommand(deviceCommand);

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

            Task.Delay(1).Wait();

            // The STATUS register value is returned at first byte on each SPI call
            LoadStatusRegister?.Invoke(new[] { receiveBuffer[0] });

            // Populate result with the rest of the receive buffer
            if (value.Length > 1 && autoRevert && _revertBytes)
                for (int i = 0; i < result.Length; i++)
                    result[i] = receiveBuffer[bufferLength - (i + 1)];
            else
                Array.Copy(receiveBuffer, 1, result, 0, bufferLength - 1);

            return result;
        }

        public byte ExecuteCommand(DeviceCommands deviceCommand, byte address)
        {
            CanExecuteCommand(deviceCommand);

            byte[] sendBuffer = new byte[1],
                    receiveBuffer = new byte[1];
            sendBuffer[0] = (byte)((byte)deviceCommand | address);
            lock (SyncRoot)
            {
                _spiDevice.TransferFullDuplex(sendBuffer, receiveBuffer);
            }
            return receiveBuffer[0];
        }

        public byte ExecuteCommand(DeviceCommands deviceCommand)
        {
            CanExecuteCommand(deviceCommand);

            byte[] sendBuffer = new byte[1],
                    receiveBuffer = new byte[1];
            sendBuffer[0] = (byte)deviceCommand;
            lock (SyncRoot)
            {
                _spiDevice.TransferFullDuplex(sendBuffer, receiveBuffer);
            }
            return receiveBuffer[0];
        }

        private void CanExecuteCommand(DeviceCommands deviceCommand)
        {
            DeviceStatus? status = GetDeviceStatus?.Invoke();
            if (CheckStatus && status.HasValue && (deviceCommand == DeviceCommands.W_REGISTER && !(status == DeviceStatus.StandBy || status == DeviceStatus.PowerDown)))
                throw new InvalidOperationException("Writing register should only in Standby or PowerDown mode");
        }
    }
}
