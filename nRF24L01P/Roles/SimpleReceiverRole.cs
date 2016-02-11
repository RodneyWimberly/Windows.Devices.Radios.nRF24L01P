using Windows.Devices.Radios.nRF24L01P.Enums;
using Windows.Devices.Radios.nRF24L01P.Interfaces;
using Windows.Devices.Radios.nRF24L01P.Registers;

namespace Windows.Devices.Radios.nRF24L01P.Roles
{
    public class SimpleReceiverRole : IRole
    {
        private Radio _radio;
        private ReceivePipe _reader;
        public byte[] Address { get; set; }
        public bool IsRunning { get; set; }
        public delegate void DataArrivedDelegate(byte[] buffer);
        public event DataArrivedDelegate DataArrived;

        public void Start()
        {
            if (IsRunning) return;
            _radio.Status = DeviceStatus.PowerDown;
            _reader = _radio.ReceivePipes[1];
            _reader.AutoAcknowledgementEnabled = true;
            _radio.Configuration.DynamicPayloadLengthEnabled = true;
            _reader.DynamicPayloadLengthEnabled = true;
            _reader.Address = Address;
            _reader.Enabled = true;
            _reader.FlushBuffer();
            _radio.TransmitPipe.FlushBuffer();
            //_radio.OnInterrupt += radio_OnInterrupt;
            _radio.Status = DeviceStatus.StandBy;
            _radio.Status = DeviceStatus.ReceiveMode;
            IsRunning = true;
        }

        private void radio_OnInterrupt(StatusRegister status)
        {
            if (status.RX_DR && DataArrived != null)
            {
                DataArrived(_reader.ReadBufferAll());
                _radio.Status = DeviceStatus.StandBy;
                status.Save();
                _radio.Status = DeviceStatus.ReceiveMode;
            }
        }

        public void Stop()
        {
            if (!IsRunning) return;
            //_radio.OnInterrupt -= radio_OnInterrupt;
            _radio.Status = DeviceStatus.StandBy;
            _radio.Status = DeviceStatus.PowerDown;
            IsRunning = false;
        }

        public void AttachDevice(Radio radio)
        {
            _radio = radio;
        }

        public void DetachDevice()
        {
            _radio = null;
        }
    }
}
