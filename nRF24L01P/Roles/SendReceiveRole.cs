using System;
using System.Threading;
using Windows.Devices.Radios.nRF24L01P.Enums;

namespace Windows.Devices.Radios.nRF24L01P.Roles
{
    public class SendReceiveRole : RoleBase
    {
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

            Writer = Radio.TransmitPipe;
            Writer.Address = SendAddress;

            Reader = Radio.ReceivePipes[1];
            Reader.AutoAcknowledgementEnabled = true;
            Reader.DynamicPayloadLengthEnabled = true;
            Reader.Address = ReceiveAddress;
            Reader.Enabled = true;

            Reader.FlushBuffer();
            Writer.FlushBuffer();

            Radio.Interrupted += Radio_Interrupted;
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
            Writer.FlushBuffer();
            Radio.Status = DeviceStatus.TransmitMode;
            while (bytesLeft > 0)
            {
                int sendBufferLength = Math.Min(bytesLeft, 32);
                if (sendBufferLength != sendBuffer.Length)
                    sendBuffer = new byte[sendBufferLength];
                Array.Copy(buffer, sendPos, sendBuffer, 0, sendBufferLength);
                sendPos += sendBufferLength;
                bytesLeft -= sendBufferLength;
                Writer.Write(sendBuffer);
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

        protected override void Radio_Interrupted(object sender, InterruptedEventArgs e)
        {
            base.Radio_Interrupted(sender, e);
            if (e.StatusRegister.MaximunTransmitRetries)
            {
                _isMaxRt = true;
                _sendCompleteEvent.Set();
            }
            if (e.StatusRegister.TransmitDataSent)
                _sendCompleteEvent.Set();
            Radio.Status = DeviceStatus.StandBy;
            if (e.StatusRegister.ReceiveDataReady)
                DataArrived?.Invoke(this, Reader.ReadBufferAll());
            e.StatusRegister.Save();
            if (!_isSending)
                Radio.Status = DeviceStatus.ReceiveMode;
        }
    }
}
