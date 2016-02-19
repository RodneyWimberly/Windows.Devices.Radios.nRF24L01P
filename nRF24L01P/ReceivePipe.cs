using System;
using Windows.Devices.Radios.nRF24L01P.Enums;
using Windows.Devices.Radios.nRF24L01P.Interfaces;
using Windows.Devices.Radios.nRF24L01P.Registers;

namespace Windows.Devices.Radios.nRF24L01P
{
    public class ReceivePipe : IReceivePipe
    {
        private readonly IRegisterManager _registerManager;
        private readonly IRadioConfiguration _configuration;
        private readonly ICommandProcessor _commandProcessor;
        public int PipeId { get; }

        public ReceivePipe(IRadioConfiguration configuration, ICommandProcessor commandProcessor, IRegisterManager registerManager, int pipeId)
        {
            if (PipeId > 5)
                throw new ArgumentOutOfRangeException(nameof(pipeId), "Invalid PipeId number for this Pipe");
            _configuration = configuration;
            _commandProcessor = commandProcessor;
            _registerManager = registerManager;
            PipeId = pipeId;
        }

        public bool ReceivedPowerDetector
        {
            get
            {
                _registerManager.ReceivedPowerDetectorRegister.Load();
                return _registerManager.ReceivedPowerDetectorRegister.ReceivedPowerDetector;
            }
        }

        public byte[] Address
        {
            get { return _registerManager.ReceiveAddressPipeRegisters[(byte)PipeId]; }
            set
            {
                int addressWidth = _configuration.AddressWidth;
                if (PipeId < 2 && value.Length < addressWidth)
                    throw new InvalidOperationException("Address length should equal or greater than device.Config.AddressWidth");
                if (PipeId < 2 && value.Length > addressWidth)
                    Array.Resize(ref value, addressWidth);
                else if (PipeId > 1 && value.Length != 1)
                    throw new InvalidOperationException("Address length should be 1 byte for receive pipes 2 to 5");
                _registerManager.ReceiveAddressPipeRegisters[(byte)PipeId].Load(value);
                _registerManager.ReceiveAddressPipeRegisters[(byte)PipeId].Save();
            }
        }

        public bool Enabled
        {
            get
            {
                switch (PipeId)
                {
                    case 0:
                        return _registerManager.EnableReceiveAddressRegister.EnableReceiveDataPipe0;
                    case 1:
                        return _registerManager.EnableReceiveAddressRegister.EnableReceiveDataPipe1;
                    case 2:
                        return _registerManager.EnableReceiveAddressRegister.EnableReceiveDataPipe2;
                    case 3:
                        return _registerManager.EnableReceiveAddressRegister.EnableReceiveDataPipe3;
                    case 4:
                        return _registerManager.EnableReceiveAddressRegister.EnableReceiveDataPipe4;
                    case 5:
                        return _registerManager.EnableReceiveAddressRegister.EnableReceiveDataPipe5;
                    default:
                        throw new InvalidOperationException("Cannot get register value, invalid ID number for this Pipe");
                }

            }
            set
            {
                switch (PipeId)
                {
                    case 0:
                        _registerManager.EnableReceiveAddressRegister.EnableReceiveDataPipe0 = value;
                        _registerManager.EnableReceiveAddressRegister.Save();
                        break;
                    case 1:
                        _registerManager.EnableReceiveAddressRegister.EnableReceiveDataPipe1 = value;
                        _registerManager.EnableReceiveAddressRegister.Save();
                        break;
                    case 2:
                        _registerManager.EnableReceiveAddressRegister.EnableReceiveDataPipe2 = value;
                        _registerManager.EnableReceiveAddressRegister.Save();
                        break;
                    case 3:
                        _registerManager.EnableReceiveAddressRegister.EnableReceiveDataPipe3 = value;
                        _registerManager.EnableReceiveAddressRegister.Save();
                        break;
                    case 4:
                        _registerManager.EnableReceiveAddressRegister.EnableReceiveDataPipe4 = value;
                        _registerManager.EnableReceiveAddressRegister.Save();
                        break;
                    case 5:
                        _registerManager.EnableReceiveAddressRegister.EnableReceiveDataPipe5 = value;
                        _registerManager.EnableReceiveAddressRegister.Save();
                        break;
                    default:
                        throw new InvalidOperationException("Cannot set register value, invalid ID number for this Pipe");
                }

            }
        }

        public bool AutoAcknowledgementEnabled
        {
            get
            {
                switch (PipeId)
                {
                    case 0:
                        return _registerManager.EnableAutoAcknowledgementRegister.EnableAutoAcknowledgementPipe0;
                    case 1:
                        return _registerManager.EnableAutoAcknowledgementRegister.EnableAutoAcknowledgementPipe1;
                    case 2:
                        return _registerManager.EnableAutoAcknowledgementRegister.EnableAutoAcknowledgementPipe2;
                    case 3:
                        return _registerManager.EnableAutoAcknowledgementRegister.EnableAutoAcknowledgementPipe3;
                    case 4:
                        return _registerManager.EnableAutoAcknowledgementRegister.EnableAutoAcknowledgementPipe4;
                    case 5:
                        return _registerManager.EnableAutoAcknowledgementRegister.EnableAutoAcknowledgementPipe5;
                    default:
                        throw new InvalidOperationException("Cannot get DynamicPayloadEnabled for data pipe, invalid ID number for this Pipe");
                }
            }
            set
            {
                switch (PipeId)
                {
                    case 0:
                        _registerManager.EnableAutoAcknowledgementRegister.EnableAutoAcknowledgementPipe0 = value;
                        _registerManager.EnableAutoAcknowledgementRegister.Save();
                        break;
                    case 1:
                        _registerManager.EnableAutoAcknowledgementRegister.EnableAutoAcknowledgementPipe1 = value;
                        _registerManager.EnableAutoAcknowledgementRegister.Save();
                        break;
                    case 2:
                        _registerManager.EnableAutoAcknowledgementRegister.EnableAutoAcknowledgementPipe2 = value;
                        _registerManager.EnableAutoAcknowledgementRegister.Save();
                        break;
                    case 3:
                        _registerManager.EnableAutoAcknowledgementRegister.EnableAutoAcknowledgementPipe3 = value;
                        _registerManager.EnableAutoAcknowledgementRegister.Save();
                        break;
                    case 4:
                        _registerManager.EnableAutoAcknowledgementRegister.EnableAutoAcknowledgementPipe4 = value;
                        _registerManager.EnableAutoAcknowledgementRegister.Save();
                        break;
                    case 5:
                        _registerManager.EnableAutoAcknowledgementRegister.EnableAutoAcknowledgementPipe5 = value;
                        _registerManager.EnableAutoAcknowledgementRegister.Save();
                        break;
                    default:
                        throw new InvalidOperationException("Cannot set DynamicPayloadEnabled for data pipe, invalid ID number for this Pipe");
                }
            }
        }

        public bool DynamicPayloadLengthEnabled
        {
            get
            {
                switch (PipeId)
                {
                    case 0:
                        return _registerManager.DynamicPayloadLengthRegister.EnableDynamicPayloadPipe0;
                    case 1:
                        return _registerManager.DynamicPayloadLengthRegister.EnableDynamicPayloadPipe1;
                    case 2:
                        return _registerManager.DynamicPayloadLengthRegister.EnableDynamicPayloadPipe2;
                    case 3:
                        return _registerManager.DynamicPayloadLengthRegister.EnableDynamicPayloadPipe3;
                    case 4:
                        return _registerManager.DynamicPayloadLengthRegister.EnableDynamicPayloadPipe4;
                    case 5:
                        return _registerManager.DynamicPayloadLengthRegister.EnableDynamicPayloadPipe5;
                    default:
                        throw new InvalidOperationException("Cannot get DynamicPayloadLengthEnabled for data pipe, invalid ID number for this Pipe");
                }
            }
            set
            {
                if (value && !_configuration.DynamicPayloadLengthEnabled)
                {
                    throw new InvalidOperationException("please enable Config.DynamicPayloadLengthEnabled before you enable this feature on data pipe");
                }
                if (value && !AutoAcknowledgementEnabled)
                {
                    throw new InvalidOperationException("please enable AutoACK of current data pipe before you can enable this feature on current data pipe");
                }
                switch (PipeId)
                {
                    case 0:
                        _registerManager.DynamicPayloadLengthRegister.EnableDynamicPayloadPipe0 = value;
                        _registerManager.DynamicPayloadLengthRegister.Save();
                        break;
                    case 1:
                        _registerManager.DynamicPayloadLengthRegister.EnableDynamicPayloadPipe1 = value;
                        _registerManager.DynamicPayloadLengthRegister.Save();
                        break;
                    case 2:
                        _registerManager.DynamicPayloadLengthRegister.EnableDynamicPayloadPipe2 = value;
                        _registerManager.DynamicPayloadLengthRegister.Save();
                        break;
                    case 3:
                        _registerManager.DynamicPayloadLengthRegister.EnableDynamicPayloadPipe3 = value;
                        _registerManager.DynamicPayloadLengthRegister.Save();
                        break;
                    case 4:
                        _registerManager.DynamicPayloadLengthRegister.EnableDynamicPayloadPipe4 = value;
                        _registerManager.DynamicPayloadLengthRegister.Save();
                        break;
                    case 5:
                        _registerManager.DynamicPayloadLengthRegister.EnableDynamicPayloadPipe5 = value;
                        _registerManager.DynamicPayloadLengthRegister.Save();
                        break;
                    default:
                        throw new InvalidOperationException("Cannot set DynamicPayloadLengthEnabled for data pipe, invalid ID number for this Pipe");
                }
            }
        }

        public byte PayloadWidth
        {
            get { return _registerManager.ReceivePayloadWidthPipeRegisters[(byte)PipeId].PayloadWidth; }
            set
            {
                _registerManager.ReceivePayloadWidthPipeRegisters[(byte)PipeId].PayloadWidth = value;
                _registerManager.ReceivePayloadWidthPipeRegisters[(byte)PipeId].Save();
            }
        }

        public byte BytesToRead => _configuration.DynamicPayloadSize;

        public FifoStatus FifoStatus
        {
            get
            {
                _registerManager.FifoStatusRegister.Load();
                if (_registerManager.FifoStatusRegister.ReceiveFifoFull)
                    return FifoStatus.Full;
                if (_registerManager.FifoStatusRegister.ReveiveFifoEmpty)
                    return FifoStatus.Empty;
                return FifoStatus.InUse;
            }
        }

        public void FlushBuffer()
        {
            _commandProcessor.ExecuteCommand(DeviceCommands.FLUSH_RX);
        }

        /// <summary>
        /// Reads data from RX buffer, use this feature when dynamic payload length is turned off
        /// Warning: use this feature will ignore the data pipe ID, it may read buffer belongs to other data pipe
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public byte[] ReadBuffer(int length)
        {
            if (length == 0 || length > 32)
                throw new ArgumentOutOfRangeException(nameof(length), "length should between 1 and 32");

            byte[] buffer = new byte[length];

            return _commandProcessor.ExecuteCommand(DeviceCommands.R_RX_PAYLOAD, RegisterAddresses.EMPTY_ADDRESS, buffer, false);
        }

        /// <summary>
        /// Reads data from RX buffer, use Dynamic payload length feature to detect package length
        /// </summary>
        /// <returns></returns>
        public byte[] ReadBufferAll(int readBufferSize = 512)
        {
            byte[] buffer = new byte[readBufferSize];
            int length = ReadBufferAll(buffer, 0);
            byte[] result = new byte[length];
            Array.Copy(buffer, result, length);
            return result;
        }

        /// <summary>
        /// reads all data available in FIFO buffer to a byte array
        /// </summary>
        /// <param name="outputBuffer">output buffer</param>
        /// <param name="writeStartPosition">start position in output buffer</param>
        /// <returns></returns>
        public int ReadBufferAll(byte[] outputBuffer, int writeStartPosition)
        {
            int position = writeStartPosition;
            int bytesRead = 0;
            if (DynamicPayloadLengthEnabled)
            {
                while (FifoStatus != FifoStatus.Empty)
                {
                    int length = BytesToRead;
                    if (length > 32)
                    {
                        FlushBuffer();
                        return 0;
                    }
                    if (length == 0)
                        return 0;
                    if (position + length > outputBuffer.Length)//buffer full
                        break;
                    Array.Copy(ReadBuffer(length), 0, outputBuffer, position, length);
                    position += length;
                    bytesRead += length;
                }
                return bytesRead;
            }
            throw new InvalidOperationException("DynamicPayloadLength is not enabled on this pipe, please turn it On or use ReadBuffer(int length) instead");
        }
    }
}
