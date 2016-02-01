namespace Windows.Devices.Radios.nRF24L01P.Registers
{
    public abstract class PipeRegisterBase : RegisterBase
    {
        public byte PipeNumber { get; }

        protected PipeRegisterBase(Radio radio, byte address, byte length, byte pipeNumber) : base(radio, length, address)
        {
            PipeNumber = pipeNumber;
        }
    }
}
