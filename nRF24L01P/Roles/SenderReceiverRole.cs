using System;
using System.Threading;
using Windows.Devices.Radios.nRF24L01P.Enums;

namespace Windows.Devices.Radios.nRF24L01P.Roles
{
    public class SenderReceiverRole : RoleBase
    {
        private bool _isSending;
        private readonly ManualResetEvent _sendCompleteEvent;
        public event EventHandler<byte[]> DataArrived;

        public SenderReceiverRole()
        {
            DataArrived = null;
            MaxRetries = false;
            _isSending = false;
            _sendCompleteEvent = new ManualResetEvent(false);
        }

        public void EnableReceivePipe(int pipeId, byte address)
        {
            if (IsRunning) throw new InvalidOperationException("Please call this method before you call the Start method");
            if (pipeId > 5 || pipeId < 2) throw new ArgumentOutOfRangeException(nameof(pipeId), "PipeId should be 2-5");
            Radio.ReceivePipes[pipeId].Address = new byte[1] { address };
            Radio.ReceivePipes[pipeId].AutoAcknowledgementEnabled = true;
            Radio.ReceivePipes[pipeId].DynamicPayloadLengthEnabled = true;
            Radio.ReceivePipes[pipeId].PayloadWidth = Radio.Configuration.PayloadWidth;
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

            Acknowledgement = Radio.ReceivePipes[0];
            Acknowledgement.AutoAcknowledgementEnabled = true;
            Acknowledgement.DynamicPayloadLengthEnabled = true;
            Acknowledgement.Address = SendAddress;
            Acknowledgement.PayloadWidth = Radio.Configuration.PayloadWidth;
            Acknowledgement.Enabled = true;

            Writer = Radio.TransmitPipe;
            Writer.Address = SendAddress;

            Reader = Radio.ReceivePipes[1];
            Reader.AutoAcknowledgementEnabled = true;
            Reader.DynamicPayloadLengthEnabled = true;
            Reader.Address = ReceiveAddress;
            Reader.PayloadWidth = Radio.Configuration.PayloadWidth;
            Reader.Enabled = true;

            Reader.FlushBuffer();
            Writer.FlushBuffer();

            Radio.Interrupted += Radio_Interrupted;
            Radio.OperatingMode = OperatingModes.ReceiveMode;

            return IsRunning = true;
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
            MaxRetries = false;
            _isSending = true;
            bool result = true;
            lock (SyncRoot)
            {
                Radio.OperatingMode = OperatingModes.StandBy;
                Writer.FlushBuffer();
                Radio.OperatingMode = OperatingModes.TransmitMode;
                while (bytesLeft > 0)
                {
                    int sendBufferLength = Math.Min(bytesLeft, Constants.MaxPayloadWidth);
                    if (sendBufferLength != sendBuffer.Length)
                        sendBuffer = new byte[sendBufferLength];
                    Array.Copy(buffer, sendPos, sendBuffer, 0, sendBufferLength);
                    sendPos += sendBufferLength;
                    bytesLeft -= sendBufferLength;

                    _sendCompleteEvent.Reset();
                    Writer.Write(sendBuffer);
                    result = _sendCompleteEvent.WaitOne(timeOut) && !MaxRetries;
                    if(!result)
                        break;
                }
                Radio.OperatingMode = OperatingModes.StandBy;
                Radio.OperatingMode = OperatingModes.ReceiveMode;
            }
            _isSending = false;
            return result;
        }

        protected override void Radio_Interrupted(object sender, InterruptedEventArgs e)
        {
            base.Radio_Interrupted(sender, e);

            //lock (SyncRoot)
            {
                MaxRetries = e.StatusRegister.MaximunTransmitRetries;
                if (MaxRetries || e.StatusRegister.TransmitDataSent)
                    _sendCompleteEvent.Set();
                if (e.StatusRegister.ReceiveDataReady)
                    DataArrived?.Invoke(this, Reader.ReadBufferAll());
                lock (SyncRoot)
                {
                    Radio.OperatingMode = OperatingModes.StandBy;
                    e.StatusRegister.ResetToDefault();
                    e.StatusRegister.Save();
                    if (!_isSending)
                        Radio.OperatingMode = OperatingModes.ReceiveMode;
                }
            }
        }
    }
}
