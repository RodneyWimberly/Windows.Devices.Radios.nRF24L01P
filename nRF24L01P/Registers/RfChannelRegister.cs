namespace Windows.Devices.Radios.nRF24L01P.Registers
{
    /// <summary>
    ///   RF Channel
    /// </summary>
    public class RfChannelRegister : RegisterBase
    {
        public RfChannelRegister(Radio radio) : base(radio, 1, Addresses.RF_CH)
        {

        }

        public byte RF_CH
        {
            get { return GetByteValue(6, Properties.RF_CH); }
            set { SetByteValue(value, 6, Properties.RF_CH); }
        }
    }
}
