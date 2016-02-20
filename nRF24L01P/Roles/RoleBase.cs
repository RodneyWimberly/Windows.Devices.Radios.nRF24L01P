using System;
using Windows.Devices.Radios.nRF24L01P.Enums;
using Windows.Devices.Radios.nRF24L01P.Interfaces;

namespace Windows.Devices.Radios.nRF24L01P.Roles
{
    public abstract class RoleBase : IRole
    {
        protected ulong[] Pipes = { 0xABCDABCD71, 0x544d52687C };

        protected IRadio Radio;
        public bool IsRunning { get; protected set; }

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

        protected IReceivePipe Reader;
        protected ITransmitPipe Writer;

        public virtual void AttachDevice(IRadio radio)
        {
            Radio = radio;
        }

        public virtual void DetachDevice()
        {
            Radio = null;
        }

        public virtual bool Start()
        {
            if (IsRunning) return false;

            Radio.Status = DeviceStatus.StandBy;

            IConfiguration config = Radio.Configuration;
            config.Channel = 1;
            config.PowerLevel = PowerLevels.Max;
            config.DataRate = DataRates.DataRate1Mbps;
            config.EnableAutoAcknowledgement = true;
            config.AutoRetransmitCount = 2;
            config.AutoRetransmitDelay = AutoRetransmitDelays.Delay4000uS;
            config.CrcEncodingScheme = CrcEncodingSchemes.SingleByte;
            config.CrcEnabled = true;
            config.DynamicPayloadLengthEnabled = true;

            Radio.Interrupted += Radio_Interrupted;
            IsRunning = true;

            return IsRunning;
        }

        protected virtual void Radio_Interrupted(object sender, InterruptedEventArgs e)
        {
        }

        public virtual void Stop()
        {
            if (!IsRunning) return;
            Radio.Interrupted -= Radio_Interrupted;
            Radio.Status = DeviceStatus.StandBy;
            Radio.Status = DeviceStatus.PowerDown;
            IsRunning = false;
        }


    }
}
