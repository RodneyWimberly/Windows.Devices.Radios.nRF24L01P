using System;
using System.Threading;
using Windows.Devices.Radios.nRF24L01P.Enums;
using Windows.Devices.Radios.nRF24L01P.Interfaces;
using Windows.Devices.Radios.nRF24L01P.Registers;

namespace Windows.Devices.Radios.nRF24L01P.Roles
{
    public class SendReceiveRole : IRole
    {
        private IRadio _radio;
        private IReceivePipe _reader;
        private ITransmitPipe _writer;
        private bool _isSending;
        private bool _isMaxRt;
        private readonly ManualResetEvent _sendCompleteEvent;
        public bool IsRunning { get; private set; }
        public delegate void DataArrivedDelegate(byte[] data);
        public event DataArrivedDelegate DataArrived;

        public SendReceiveRole()
        {
            IsRunning = false;
            DataArrived = null;
            _isMaxRt = false;
            _isSending = false;
            _sendCompleteEvent = new ManualResetEvent(false);
        }

        public void AttachDevice(IRadio radio)
        {
            _radio = radio;
        }

        public void DetachDevice()
        {
            _radio = null;
        }

        private byte[] _sendAddress;

        public byte[] SendAddress
        {
            get { return _sendAddress; }
            set
            {
                if (IsRunning) throw new InvalidOperationException("SendAddress is read-only while Running");
                _sendAddress = value;
            }
        }

        private byte[] _receiveAddress;

        public byte[] ReceiveAddress
        {
            get { return _receiveAddress; }
            set
            {
                if (IsRunning) throw new InvalidOperationException("ReceiveAddress is read-only while Running");
                _receiveAddress = value;
            }
        }

        public void EnableReceivePipe(int pipeId, byte address)
        {
            if (IsRunning) throw new InvalidOperationException("Please call this method before you call the Start method");
            if (pipeId > 5 || pipeId < 2) throw new ArgumentOutOfRangeException(nameof(pipeId), "pipeId should be 2-5");
            _radio.ReceivePipes[pipeId].Address = new byte[1] { address };
            _radio.ReceivePipes[pipeId].AutoAcknowledgementEnabled = true;
            _radio.ReceivePipes[pipeId].DynamicPayloadLengthEnabled = true;
            _radio.ReceivePipes[pipeId].Enabled = true;
        }

        public void DisableReceivePipe(int pipeId)
        {
            if (IsRunning) throw new InvalidOperationException("Please call this method before you call the Start method");
            if (pipeId > 5 || pipeId < 2) throw new ArgumentOutOfRangeException(nameof(pipeId), "pipeId should be 2-5");
            _radio.ReceivePipes[pipeId].Enabled = false;
        }

        public void Start()
        {
            if (IsRunning) return;
            if (SendAddress == null || ReceiveAddress == null) throw new InvalidOperationException("Please set the SendAddress and ReceiveAddress before Start");
            _radio.Status = DeviceStatus.StandBy;
            _radio.Configuration.DynamicPayloadLengthEnabled = true;
            _radio.ReceivePipes[0].AutoAcknowledgementEnabled = true;
            _radio.ReceivePipes[0].DynamicPayloadLengthEnabled = true;
            _radio.ReceivePipes[0].Address = this.SendAddress;
            _radio.ReceivePipes[0].Enabled = true;
            _writer = _radio.TransmitPipe;
            _writer.Address = SendAddress;
            _reader = _radio.ReceivePipes[1];
            _reader.AutoAcknowledgementEnabled = true;
            _reader.DynamicPayloadLengthEnabled = true;
            _reader.Address = this.ReceiveAddress;
            _reader.Enabled = true;
            _reader.FlushBuffer();
            _writer.FlushBuffer();
            //_radio.OnInterrupt += radio_OnInterrupt;
            _radio.Status = DeviceStatus.ReceiveMode;
            IsRunning = true;
        }

        /// <summary>
        /// send data target
        /// </summary>
        /// <param name="buffer">data buffer</param>
        /// <param name="timeOut">time out in millium seconds</param>
        /// <returns></returns>
        public bool Send(byte[] buffer, int timeOut = 1000)
        {
            if (!IsRunning) throw new InvalidOperationException("Please call [Start] method before send data");
            byte[] sendBuffer = new byte[32];
            int bytesLeft = buffer.Length;
            int sendPos = 0;
            bool result = true;
            _isMaxRt = false;
            _isSending = true;
            int length = buffer.Length;
            _radio.Status = DeviceStatus.StandBy;
            _writer.FlushBuffer();
            _radio.Status = DeviceStatus.TransmitMode;
            while (bytesLeft > 0)
            {
                int sendBufferLength = Math.Min(bytesLeft, 32);
                if (sendBufferLength != sendBuffer.Length)
                    sendBuffer = new byte[sendBufferLength];
                Array.Copy(buffer, sendPos, sendBuffer, 0, sendBufferLength);
                sendPos += sendBufferLength;
                bytesLeft -= sendBufferLength;
                _writer.Write(sendBuffer);
                _sendCompleteEvent.Reset();
                if (!(_sendCompleteEvent.WaitOne(timeOut) && !_isMaxRt))
                {
                    result = false;
                    break;
                }
            }
            _radio.Status = DeviceStatus.StandBy;
            _radio.Status = DeviceStatus.ReceiveMode;
            _isSending = false;
            return result;
        }

        private void radio_OnInterrupt(StatusRegister status)
        {
            if (status.MaximunTransmitRetries)
            {
                _isMaxRt = true;
                _sendCompleteEvent.Set();
            }
            if (status.TransmitDataSent)
                _sendCompleteEvent.Set();
            _radio.Status = DeviceStatus.StandBy;
            if (status.ReceiveDataReady)
                DataArrived?.Invoke(_reader.ReadBufferAll());
            status.Save();
            if (!_isSending)
                _radio.Status = DeviceStatus.ReceiveMode;
        }

        public void Stop()
        {
            if (!IsRunning) return;
            //_radio.OnInterrupt -= radio_OnInterrupt;
            _radio.Status = DeviceStatus.StandBy;
            _radio.Status = DeviceStatus.PowerDown;
            IsRunning = false;
        }
    }
}
