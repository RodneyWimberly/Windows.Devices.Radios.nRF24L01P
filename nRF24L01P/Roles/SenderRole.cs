using System;
using System.Threading;
using Windows.Devices.Radios.nRF24L01P.Enums;

namespace Windows.Devices.Radios.nRF24L01P.Roles
{
    public class SenderRole : RoleBase
    {

        private readonly ManualResetEvent _sentEvent;

        public SenderRole()
        {
            MaxRetries = false;
            _sentEvent = new ManualResetEvent(false);
        }

        public override bool Start()
        {
            if (!base.Start()) return false;

            Acknowledgement = Radio.ReceivePipes[0];
            Acknowledgement.AutoAcknowledgementEnabled = true;
            Acknowledgement.DynamicPayloadLengthEnabled = true;
            Acknowledgement.Address = SendAddress;
            Acknowledgement.PayloadWidth = Radio.Configuration.PayloadWidth;
            Acknowledgement.Enabled = true;

            Writer = Radio.TransmitPipe;
            Writer.Address = SendAddress;

            Acknowledgement.FlushBuffer();
            Writer.FlushBuffer();

            Radio.Interrupted += Radio_Interrupted;
            Radio.OperatingMode = OperatingModes.TransmitMode;

            return IsRunning = true;
        }

        protected override void Radio_Interrupted(object sender, InterruptedEventArgs e)
        {
            if (!e.StatusRegister.TransmitDataSent && !e.StatusRegister.MaximunTransmitRetries) return;
            base.Radio_Interrupted(sender, e);

            Writer.FlushBuffer();
            MaxRetries = e.StatusRegister.MaximunTransmitRetries;
            lock (SyncRoot)
            {
                Radio.OperatingMode = OperatingModes.StandBy;
                e.StatusRegister.ResetToDefault();
                e.StatusRegister.Save();
                Radio.OperatingMode = OperatingModes.TransmitMode;
                _sentEvent.Set();
            }
        }

        public void SendAsync(byte[] buffer)
        {
            if (buffer.Length > 32) throw new ArgumentOutOfRangeException(nameof(buffer), "Buffer should less than 32 bytes");
            Writer.Write(buffer);
        }

        public bool Send(byte[] buffer, int timeOut = 1000)
        {
            if (buffer.Length > 32) throw new ArgumentOutOfRangeException(nameof(buffer), "Buffer should less than 32 bytes");
            _sentEvent.Reset();
            lock (SyncRoot)
            {
                SendAsync(buffer);
            }
            return _sentEvent.WaitOne(timeOut) && !MaxRetries;
        }
    }
}
