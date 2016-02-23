using System;
using Windows.Devices.Radios.nRF24L01P.Enums;
using Windows.Devices.Radios.nRF24L01P.Interfaces;
using Windows.Devices.Radios.nRF24L01P.Registers;

namespace Windows.Devices.Radios.nRF24L01P
{
    public class ReceivePipe : IReceivePipe
    {
        private readonly IRegisterContainer _registerContainer;
        private readonly IConfiguration _configuration;
        private readonly ICommandProcessor _commandProcessor;
        private readonly IReceivePipeCollection _parent;
        public int PipeId { get; }

        public ReceivePipe(IConfiguration configuration, ICommandProcessor commandProcessor, IRegisterContainer registerContainer, IReceivePipeCollection parent, int pipeId)
        {
            if (PipeId > 5)
                throw new ArgumentOutOfRangeException(nameof(pipeId), "Invalid PipeId number for this Pipe");
            _configuration = configuration;
            _commandProcessor = commandProcessor;
            _registerContainer = registerContainer;
            _parent = parent;
            PipeId = pipeId;
        }

        public byte[] Address
        {
            get { return _registerContainer.ReceiveAddressPipeRegisters[(byte)PipeId]; }
            set
            {
                int addressWidth = _configuration.AddressWidth;
                if (PipeId < 2 && value.Length < addressWidth)
                    throw new InvalidOperationException("Address length should equal or greater than device.Config.AddressWidth");
                if (PipeId < 2 && value.Length > addressWidth)
                    Array.Resize(ref value, addressWidth);
                else if (PipeId > 1 && value.Length != 1)
                    throw new InvalidOperationException("Address length should be 1 byte for receive pipes 2 to 5");
                _registerContainer.ReceiveAddressPipeRegisters[(byte)PipeId].Load(value);
                _registerContainer.ReceiveAddressPipeRegisters[(byte)PipeId].Save();
            }
        }

        public bool Enabled
        {
            get
            {
                switch (PipeId)
                {
                    case 0:
                        return _registerContainer.EnableReceiveAddressRegister.EnableReceiveDataPipe0;
                    case 1:
                        return _registerContainer.EnableReceiveAddressRegister.EnableReceiveDataPipe1;
                    case 2:
                        return _registerContainer.EnableReceiveAddressRegister.EnableReceiveDataPipe2;
                    case 3:
                        return _registerContainer.EnableReceiveAddressRegister.EnableReceiveDataPipe3;
                    case 4:
                        return _registerContainer.EnableReceiveAddressRegister.EnableReceiveDataPipe4;
                    case 5:
                        return _registerContainer.EnableReceiveAddressRegister.EnableReceiveDataPipe5;
                    default:
                        throw new InvalidOperationException("Cannot get register value, invalid ID number for this Pipe");
                }

            }
            set
            {
                switch (PipeId)
                {
                    case 0:
                        _registerContainer.EnableReceiveAddressRegister.EnableReceiveDataPipe0 = value;
                        break;
                    case 1:
                        _registerContainer.EnableReceiveAddressRegister.EnableReceiveDataPipe1 = value;
                        break;
                    case 2:
                        _registerContainer.EnableReceiveAddressRegister.EnableReceiveDataPipe2 = value;
                        break;
                    case 3:
                        _registerContainer.EnableReceiveAddressRegister.EnableReceiveDataPipe3 = value;
                        break;
                    case 4:
                        _registerContainer.EnableReceiveAddressRegister.EnableReceiveDataPipe4 = value;
                        break;
                    case 5:
                        _registerContainer.EnableReceiveAddressRegister.EnableReceiveDataPipe5 = value;
                        break;
                    default:
                        throw new InvalidOperationException("Cannot set register value, invalid ID number for this Pipe");
                }
                _registerContainer.EnableReceiveAddressRegister.Save();
            }
        }

        public bool AutoAcknowledgementEnabled
        {
            get
            {
                switch (PipeId)
                {
                    case 0:
                        return _registerContainer.EnableAutoAcknowledgementRegister.EnableAutoAcknowledgementPipe0;
                    case 1:
                        return _registerContainer.EnableAutoAcknowledgementRegister.EnableAutoAcknowledgementPipe1;
                    case 2:
                        return _registerContainer.EnableAutoAcknowledgementRegister.EnableAutoAcknowledgementPipe2;
                    case 3:
                        return _registerContainer.EnableAutoAcknowledgementRegister.EnableAutoAcknowledgementPipe3;
                    case 4:
                        return _registerContainer.EnableAutoAcknowledgementRegister.EnableAutoAcknowledgementPipe4;
                    case 5:
                        return _registerContainer.EnableAutoAcknowledgementRegister.EnableAutoAcknowledgementPipe5;
                    default:
                        throw new InvalidOperationException("Cannot get DynamicPayloadEnabled for data pipe, invalid ID number for this Pipe");
                }
            }
            set
            {
                switch (PipeId)
                {
                    case 0:
                        _registerContainer.EnableAutoAcknowledgementRegister.EnableAutoAcknowledgementPipe0 = value;
                        break;
                    case 1:
                        _registerContainer.EnableAutoAcknowledgementRegister.EnableAutoAcknowledgementPipe1 = value;
                        break;
                    case 2:
                        _registerContainer.EnableAutoAcknowledgementRegister.EnableAutoAcknowledgementPipe2 = value;
                        break;
                    case 3:
                        _registerContainer.EnableAutoAcknowledgementRegister.EnableAutoAcknowledgementPipe3 = value;
                        break;
                    case 4:
                        _registerContainer.EnableAutoAcknowledgementRegister.EnableAutoAcknowledgementPipe4 = value;
                        break;
                    case 5:
                        _registerContainer.EnableAutoAcknowledgementRegister.EnableAutoAcknowledgementPipe5 = value;
                        break;
                    default:
                        throw new InvalidOperationException("Cannot set DynamicPayloadEnabled for data pipe, invalid ID number for this Pipe");
                }
                _registerContainer.EnableAutoAcknowledgementRegister.Save();
            }
        }

        public bool DynamicPayloadLengthEnabled
        {
            get
            {
                switch (PipeId)
                {
                    case 0:
                        return _registerContainer.DynamicPayloadLengthRegister.EnableDynamicPayloadPipe0;
                    case 1:
                        return _registerContainer.DynamicPayloadLengthRegister.EnableDynamicPayloadPipe1;
                    case 2:
                        return _registerContainer.DynamicPayloadLengthRegister.EnableDynamicPayloadPipe2;
                    case 3:
                        return _registerContainer.DynamicPayloadLengthRegister.EnableDynamicPayloadPipe3;
                    case 4:
                        return _registerContainer.DynamicPayloadLengthRegister.EnableDynamicPayloadPipe4;
                    case 5:
                        return _registerContainer.DynamicPayloadLengthRegister.EnableDynamicPayloadPipe5;
                    default:
                        throw new InvalidOperationException("Cannot get DynamicPayloadLengthEnabled for data pipe, invalid ID number for this Pipe");
                }
            }
            set
            {
                if (value && !_configuration.DynamicPayloadLengthEnabled)
                {
                    throw new InvalidOperationException("Please enable the global DynamicPayloadLengthEnabled flag before you enable this feature on data pipe");
                }
                if (value && !AutoAcknowledgementEnabled)
                {
                    throw new InvalidOperationException("Please enable AutoAcknowledgement of current data pipe before you can enable Dynamic Payload Length");
                }
                switch (PipeId)
                {
                    case 0:
                        _registerContainer.DynamicPayloadLengthRegister.EnableDynamicPayloadPipe0 = value;
                        break;
                    case 1:
                        _registerContainer.DynamicPayloadLengthRegister.EnableDynamicPayloadPipe1 = value;
                        break;
                    case 2:
                        _registerContainer.DynamicPayloadLengthRegister.EnableDynamicPayloadPipe2 = value;
                        break;
                    case 3:
                        _registerContainer.DynamicPayloadLengthRegister.EnableDynamicPayloadPipe3 = value;
                        break;
                    case 4:
                        _registerContainer.DynamicPayloadLengthRegister.EnableDynamicPayloadPipe4 = value;
                        break;
                    case 5:
                        _registerContainer.DynamicPayloadLengthRegister.EnableDynamicPayloadPipe5 = value;
                        break;
                    default:
                        throw new InvalidOperationException("Cannot set DynamicPayloadLengthEnabled for data pipe, invalid ID number for this Pipe");
                }
                _registerContainer.DynamicPayloadLengthRegister.Save();
            }
        }

        public byte PayloadWidth
        {
            get { return _registerContainer.ReceivePayloadWidthPipeRegisters[(byte)PipeId].PayloadWidth; }
            set
            {
                _registerContainer.ReceivePayloadWidthPipeRegisters[(byte)PipeId].PayloadWidth = value;
                _registerContainer.ReceivePayloadWidthPipeRegisters[(byte)PipeId].Save();
            }
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
                throw new ArgumentOutOfRangeException(nameof(length), "Length should between 1 and 32");

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
            if (!DynamicPayloadLengthEnabled)
                throw new InvalidOperationException("DynamicPayloadLength is not enabled on this pipe, please turn it On or use ReadBuffer(int length) instead");
            while (_parent.FifoStatus != FifoStatus.Empty)
            {
                int length = BytesToRead;
                if (length > 32)
                {
                    _parent.FlushBuffer();
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

        public byte BytesToRead => _configuration.DynamicPayloadSize;

        public FifoStatus FifoStatus => _parent.FifoStatus;

        public void FlushBuffer() => _parent.FlushBuffer();
    }
}
