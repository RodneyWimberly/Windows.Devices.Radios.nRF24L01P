using System;
using Windows.Devices.Radios.nRF24L01P.Enums;
using Windows.Devices.Radios.nRF24L01P.Interfaces;
using Windows.Devices.Radios.nRF24L01P.Registers;

namespace Windows.Devices.Radios.nRF24L01P
{
    public class TransmitPipe : ITransmitPipe
    {
        private readonly IRegisterManager _registerManager;
        private readonly IRadioConfiguration _configuration;
        private readonly ICommandProcessor _commandProcessor;

        public TransmitPipe(IRadioConfiguration configuration, ICommandProcessor commandProcessor, IRegisterManager registerManager)
        {
            _configuration = configuration;
            _commandProcessor = commandProcessor;
            _registerManager = registerManager;
        }

        public byte[] Address
        {
            get
            {
                return _registerManager.TransmitAddressRegister;
            }
            set
            {
                int addressWidth = _configuration.AddressWidth;
                if (value.Length < addressWidth)
                    throw new InvalidOperationException("Address length should equal or greater than device.Config.AddressWidth");
                if (value.Length > addressWidth)
                    Array.Resize(ref value, addressWidth);
                _registerManager.TransmitAddressRegister.Load(value);
                _registerManager.TransmitAddressRegister.Save();
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
                _registerManager.FifoStatusRegister.Load();
                if (_registerManager.FifoStatusRegister.TransmitFifoFull)
                    return FifoStatus.Full;
                if (_registerManager.FifoStatusRegister.TransmitFifoEmpty)
                    return FifoStatus.Empty;
                return FifoStatus.InUse;
            }
        }

        public void Write(byte[] data, bool disableACK = false)
        {
            if (data.Length > Constants.MaxPayloadWidth)
                throw new ArgumentOutOfRangeException(nameof(data), string.Format("data should be 0-{0} bytes", Constants.MaxPayloadWidth));

            _commandProcessor.ExecuteCommand(disableACK ? DeviceCommands.W_TX_PAYLOAD_NO_ACK : DeviceCommands.W_TX_PAYLOAD, RegisterAddresses.EMPTY_ADDRESS, data);
        }
    }
}
