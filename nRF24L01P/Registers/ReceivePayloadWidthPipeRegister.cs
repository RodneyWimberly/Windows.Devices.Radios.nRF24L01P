namespace Windows.Devices.Radios.nRF24L01P.Registers
{
    /// <summary>
    ///   Number of bytes in RX payload in data pipe 
    /// </summary>
    public class ReceivePayloadWidthPipeRegister : PipeRegisterBase
    {
        public ReceivePayloadWidthPipeRegister(Radio radio, byte address, byte pipeNumber) :
            base(radio, address, 1, pipeNumber)
        { }

        public byte PayloadWidth
        {
            get { return GetByteValue(5, Properties.RX_PW_P0); }
            set { SetByteValue(value, 5, Properties.RX_PW_P0); }
        }
    }
}
