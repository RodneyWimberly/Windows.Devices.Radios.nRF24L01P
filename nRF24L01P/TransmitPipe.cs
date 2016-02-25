using System;
using System.Diagnostics;
using Windows.Devices.Gpio;
using Windows.Devices.Radios.nRF24L01P.Enums;
using Windows.Devices.Radios.nRF24L01P.Interfaces;
using Windows.Devices.Radios.nRF24L01P.Registers;

namespace Windows.Devices.Radios.nRF24L01P
{
    public class TransmitPipe : ITransmitPipe
    {
        private readonly IRegisterContainer _registerContainer;
        private readonly IConfiguration _configuration;
        private readonly ICommandProcessor _commandProcessor;
        private readonly GpioPin _cePin;

        public TransmitPipe(IConfiguration configuration, ICommandProcessor commandProcessor, IRegisterContainer registerContainer, GpioPin cePin)
        {
            _configuration = configuration;
            _commandProcessor = commandProcessor;
            _registerContainer = registerContainer;
            _cePin = cePin;
        }

        public byte[] Address
        {
            get
            {
                return _registerContainer.TransmitAddressRegister;
            }
            set
            {
                int addressWidth = _configuration.AddressWidth;
                if (value.Length < addressWidth)
                    throw new InvalidOperationException("Address length should equal or greater than device.Config.AddressWidth");
                if (value.Length > addressWidth)
                    Array.Resize(ref value, addressWidth);
                _registerContainer.TransmitAddressRegister.Load(value);
                _registerContainer.TransmitAddressRegister.Save();
            }
        }

        public void FlushBuffer()
        {
            _commandProcessor.ExecuteCommand(DeviceCommands.FLUSH_TX);
        }

        public FifoStatus FifoStatus
        {
            get
            {
                FifoStatus fifoStatus = FifoStatus.InUse;
                _registerContainer.FifoStatusRegister.Load();
                if (_registerContainer.FifoStatusRegister.TransmitFifoFull)
                    fifoStatus = FifoStatus.Full;
                else if (_registerContainer.FifoStatusRegister.TransmitFifoEmpty)
                    fifoStatus = FifoStatus.Empty;
                return fifoStatus;
            }
        }

        public void ReuseTransmitPayload()
        {
            _registerContainer.StatusRegister.ResetToDefault();
            _cePin.Write(GpioPinValue.Low);
            _cePin.Write(GpioPinValue.High);
        }

        public bool Write(byte[] data, bool disableAck = false, int timeout = 1000)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            StatusRegister statusRegister = _registerContainer.StatusRegister;

            while (statusRegister.TransmitFifoFull)
            {
                if (statusRegister.MaximunTransmitRetries)
                {
                    //ReuseTransmitPayload();
                    bool checkOperatingMode = _commandProcessor.CheckOperatingMode;
                    _commandProcessor.CheckOperatingMode = false;

                    statusRegister.ResetToDefault();
                    _commandProcessor.CheckOperatingMode = checkOperatingMode;
                    return false;
                }
                //if (stopwatch.ElapsedMilliseconds() > timeout)
                //    return false;
                statusRegister.Load();
            }
            if (data.Length > Constants.MaxPayloadWidth)
                throw new ArgumentOutOfRangeException(nameof(data), string.Format("Data should be 0-{0} bytes", Constants.MaxPayloadWidth));
            _commandProcessor.ExecuteCommand(disableAck ? DeviceCommands.W_TX_PAYLOAD_NO_ACK : DeviceCommands.W_TX_PAYLOAD, RegisterAddresses.EMPTY_ADDRESS, data);
            return true;
        }
    }
}
