using Windows.Devices.Radios.nRF24L01P.Enums;

namespace Windows.Devices.Radios.nRF24L01P.Roles
{
    public class ConstantCarrierWaveRole : RoleBase
    {
        public override bool Start()
        {
            if (IsRunning) return true;
            if (!base.Start()) return false;

            Radio.Configuration.EnableContiuousCarrierTransmit = true;
            Radio.Status = DeviceStatus.TransmitMode;

            return IsRunning;
        }

        public override void Stop()
        {
            if (!IsRunning) return;
            Radio.Configuration.EnableContiuousCarrierTransmit = false;
            base.Stop();
        }
    }
}
