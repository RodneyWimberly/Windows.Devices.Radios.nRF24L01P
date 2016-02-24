using System;
using System.Threading;
using Windows.Devices.Radios.nRF24L01P.Enums;

namespace Windows.Devices.Radios.nRF24L01P.Roles
{
    public class SimpleSendRole : RoleBase
    {

        private readonly ManualResetEvent _sentEvent;
        private bool _maxRetries;

        public SimpleSendRole()
        {
            _maxRetries = false;
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
            _maxRetries = e.StatusRegister.MaximunTransmitRetries;
            Radio.OperatingMode = OperatingModes.StandBy;
            e.StatusRegister.Save();
            Radio.OperatingMode = OperatingModes.TransmitMode;
            _sentEvent.Set();
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
            SendAsync(buffer);
            return _sentEvent.WaitOne(timeOut) && !_maxRetries;
        }
    }
}
