using System;
using Windows.Devices.Radios.nRF24L01P.Enums;
using Windows.Devices.Radios.nRF24L01P.Interfaces;

namespace Windows.Devices.Radios.nRF24L01P.Roles
{
    public abstract class RoleBase : IRole
    {
        protected IRadio Radio;
        public bool IsRunning { get; protected set; }
        public bool MaxRetries { get; protected set; }

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

        protected IReceivePipe Acknowledgement;
        protected IReceivePipe Reader;
        protected ITransmitPipe Writer;
        protected object SyncRoot;

        protected RoleBase()
        {
            SyncRoot = new object();
        }

        public virtual void AttachRadio(IRadio radio)
        {
            Radio = radio;
        }

        public virtual void DetachRadio()
        {
            Radio = null;
        }

        public virtual bool Start()
        {
            if (IsRunning) return false;

            Radio.OperatingMode = OperatingModes.StandBy;

            IConfiguration config = Radio.Configuration;
            config.Channel = 1;
            config.PowerLevel = PowerLevels.Max;
            config.DataRate = DataRates.DataRate1Mbps;
            config.EnableAutoAcknowledgement = true;
            config.AutoRetransmitCount = 15;
            config.AutoRetransmitDelay = AutoRetransmitDelays.Delay4000uS;
            config.CrcEncodingScheme = CrcEncodingSchemes.SingleByte;
            config.DynamicPayloadLengthEnabled = true;

            Radio.RegisterContainer.FeatureRegister.EnablePayloadWithAcknowledgement = true;
            Radio.RegisterContainer.FeatureRegister.EnableTransmitWithPayloadAcknowledgement = true;
            Radio.RegisterContainer.FeatureRegister.Save();

            return IsRunning = true;
        }

        protected virtual void Radio_Interrupted(object sender, InterruptedEventArgs e)
        {
        }

        public virtual void Stop()
        {
            if (!IsRunning) return;
            Radio.Interrupted -= Radio_Interrupted;
            Radio.OperatingMode = OperatingModes.StandBy;
            Radio.OperatingMode = OperatingModes.PowerDown;
            IsRunning = false;
        }
    }
}
