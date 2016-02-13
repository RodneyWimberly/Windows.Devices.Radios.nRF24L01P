using System;
using System.Threading;
using Windows.Devices.Radios.nRF24L01P.Enums;
using Windows.Devices.Radios.nRF24L01P.Interfaces;
using Windows.Devices.Radios.nRF24L01P.Registers;

namespace Windows.Devices.Radios.nRF24L01P.Roles
{
    public class SimpleSendRole : IRole
    {
        private IRadio _radio;

        private IReceivePipe _reader;
        private ITransmitPipe _writer;
        private readonly ManualResetEvent _sentEvent;
        private bool _maxRt = false;
        public byte[] Address { get; set; }
        public bool IsRunning { get; private set; }

        public SimpleSendRole()
        {
            _maxRt = false;
            _sentEvent = new ManualResetEvent(false);
        }

        public void Start()
        {
            if (IsRunning) return;
            _radio.Status = DeviceStatus.PowerDown;
            _reader = _radio.ReceivePipes[0];
            _writer = _radio.TransmitPipe;
            _radio.Configuration.DynamicPayloadLengthEnabled = true;

            _reader.AutoAcknowledgementEnabled = true;
            _reader.DynamicPayloadLengthEnabled = true;
            _reader.Address = Address;
            _reader.Enabled = true;

            _writer.Address = Address;

            _reader.FlushBuffer();
            _writer.FlushBuffer();

            //_radio.OnInterrupt += device_OnInterrupt;

            _radio.Status = DeviceStatus.StandBy;
            _radio.Status = DeviceStatus.TransmitMode;
            IsRunning = true;
        }

        private void radio_OnInterrupt(StatusRegister status)
        {
            if (status.TransmitDataSent || status.MaximunTransmitRetries)// data sent
            {
                _writer.FlushBuffer();
                _maxRt = status.MaximunTransmitRetries;
                _radio.Status = DeviceStatus.StandBy;
                status.Save();
                _radio.Status = DeviceStatus.TransmitMode;
                _sentEvent.Set();
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

        public void AttachDevice(IRadio radio)
        {
            _radio = radio;
        }

        public void DetachDevice()
        {
            _radio = null;
        }

        public void SendAsync(byte[] buffer)
        {
            if (buffer.Length > 32) throw new ArgumentOutOfRangeException(nameof(buffer), "Buffer should less than 32 bytes");
            _writer.Write(buffer);
        }

        public bool Send(byte[] buffer, int timeOut = 1000)
        {
            if (buffer.Length > 32) throw new ArgumentOutOfRangeException(nameof(buffer), "Buffer should less than 32 bytes");
            _sentEvent.Reset();
            SendAsync(buffer);
            return _sentEvent.WaitOne(timeOut) && !_maxRt;
        }
    }
}
