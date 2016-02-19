using Windows.Devices.Radios.nRF24L01P.Enums;
using Windows.Devices.Radios.nRF24L01P.Interfaces;

namespace Windows.Devices.Radios.nRF24L01P.Roles
{
    public abstract class RoleBase : IRole
    {
        protected IRadio Radio;
        public bool IsRunning { get; protected set; }

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

            IRadioConfiguration config = Radio.Configuration;
            config.Channel = 1;
            config.PowerLevel = PowerLevels.Max;
            config.DataRate = DataRates.DataRate1Mbps;
            config.AutoRetransmitCount = 2;
            config.AutoRetransmitDelay = AutoRetransmitDelays.Delay4000uS;
            config.CrcEncodingScheme = CrcEncodingSchemes.SingleByte;
            config.EnableAutoAcknowledgement = true;
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
