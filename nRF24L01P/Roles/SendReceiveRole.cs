using System;
using System.Threading;
using Windows.Devices.Radios.nRF24L01P.Enums;
using Windows.Devices.Radios.nRF24L01P.Interfaces;

namespace Windows.Devices.Radios.nRF24L01P.Roles
{
    public class SendReceiveRole : RoleBase
    {
        private IReceivePipe _reader;
        private ITransmitPipe _writer;
        private bool _isSending;
        private bool _isMaxRt;
        private readonly ManualResetEvent _sendCompleteEvent;
        public event EventHandler<byte[]> DataArrived;

        public SendReceiveRole()
        {
            DataArrived = null;
            _isMaxRt = false;
            _isSending = false;
            _sendCompleteEvent = new ManualResetEvent(false);
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
            Radio.ReceivePipes[pipeId].Address = new byte[1] { address };
            Radio.ReceivePipes[pipeId].AutoAcknowledgementEnabled = true;
            Radio.ReceivePipes[pipeId].DynamicPayloadLengthEnabled = true;
            Radio.ReceivePipes[pipeId].Enabled = true;
        }

        public void DisableReceivePipe(int pipeId)
        {
            if (IsRunning) throw new InvalidOperationException("Please call this method before you call the Start method");
            if (pipeId > 5 || pipeId < 2) throw new ArgumentOutOfRangeException(nameof(pipeId), "pipeId should be 2-5");
            Radio.ReceivePipes[pipeId].Enabled = false;
        }

        public override bool Start()
        {
            if (SendAddress == null || ReceiveAddress == null) throw new InvalidOperationException("Please set the SendAddress and ReceiveAddress before Start");

            if (!base.Start()) return false;

            Radio.ReceivePipes[0].AutoAcknowledgementEnabled = true;
            Radio.ReceivePipes[0].DynamicPayloadLengthEnabled = true;
            Radio.ReceivePipes[0].Address = SendAddress;
            Radio.ReceivePipes[0].Enabled = true;

            _writer = Radio.TransmitPipe;
            _writer.Address = SendAddress;

            _reader = Radio.ReceivePipes[1];
            _reader.AutoAcknowledgementEnabled = true;
            _reader.DynamicPayloadLengthEnabled = true;
            _reader.Address = ReceiveAddress;
            _reader.Enabled = true;

            _reader.FlushBuffer();
            _writer.FlushBuffer();

            Radio.OnInterrupt += Radio_OnInterrupt;
            Radio.Status = DeviceStatus.ReceiveMode;
            IsRunning = true;

            return IsRunning;
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
            Radio.Status = DeviceStatus.StandBy;
            _writer.FlushBuffer();
            Radio.Status = DeviceStatus.TransmitMode;
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
            Radio.Status = DeviceStatus.StandBy;
            Radio.Status = DeviceStatus.ReceiveMode;
            _isSending = false;
            return result;
        }

        protected override void Radio_OnInterrupt(object sender, Registers.StatusRegister e)
        {
            base.Radio_OnInterrupt(sender, e);
            if (e.MaximunTransmitRetries)
            {
                _isMaxRt = true;
                _sendCompleteEvent.Set();
            }
            if (e.TransmitDataSent)
                _sendCompleteEvent.Set();
            Radio.Status = DeviceStatus.StandBy;
            if (e.ReceiveDataReady)
                DataArrived?.Invoke(this, _reader.ReadBufferAll());
            e.Save();
            if (!_isSending)
                Radio.Status = DeviceStatus.ReceiveMode;
        }
    }
}
