namespace Windows.Devices.Radios.nRF24L01.Registers
{
    public class StatusRegister : RegisterBase
    {
        public StatusRegister(Radio radio) : base(radio, 1, Constants.STATUS)
        {
        }

        public bool RX_DR
        {
            get { return GetBitValue(Constants.RX_DR); }
            set { FirstByte = SetBitValue(Constants.RX_DR, value); }
        }

        public bool TX_DS
        {
            get { return GetBitValue(Constants.TX_DS); }
            set { FirstByte = SetBitValue(Constants.TX_DS, value); }
        }

        public bool MAX_RT
        {
            get { return GetBitValue(Constants.MAX_RT); }
            set { FirstByte = SetBitValue(Constants.MAX_RT, value); }
        }

        public byte RX_P_NO => GetByteValue(3, 1);

        public bool TX_FULL => GetBitValue(Constants.TX_FULL);
    }
}
