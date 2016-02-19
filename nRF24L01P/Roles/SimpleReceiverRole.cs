using System;
using Windows.Devices.Radios.nRF24L01P.Enums;
using Windows.Devices.Radios.nRF24L01P.Interfaces;

namespace Windows.Devices.Radios.nRF24L01P.Roles
{
    public class SimpleReceiverRole : RoleBase
    {
        private IReceivePipe _reader;
        public byte[] Address { get; set; }
        public event EventHandler<byte[]> DataArrived;

        public override bool Start()
        {
            if (!base.Start()) return false;

            _reader = Radio.ReceivePipes[1];
            _reader.AutoAcknowledgementEnabled = true;
            _reader.DynamicPayloadLengthEnabled = true;
            _reader.Address = Address;
            _reader.Enabled = true;

            _reader.FlushBuffer();
            Radio.TransmitPipe.FlushBuffer();

            Radio.Interrupted += Radio_Interrupted;
            Radio.Status = DeviceStatus.StandBy;
            Radio.Status = DeviceStatus.ReceiveMode;
            IsRunning = true;

            return IsRunning;
        }

        protected override void Radio_Interrupted(object sender, InterruptedEventArgs e)
        {
            if (e.StatusRegister.ReceiveDataReady && DataArrived != null)
            {
                DataArrived(this, _reader.ReadBufferAll());
                Radio.Status = DeviceStatus.StandBy;
                e.StatusRegister.Save();
                Radio.Status = DeviceStatus.ReceiveMode;
            }
        }
    }
}
