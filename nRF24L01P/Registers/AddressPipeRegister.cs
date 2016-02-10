namespace Windows.Devices.Radios.nRF24L01P.Registers
{
    /// <summary>
    ///   Transmit/Receive address data pipe. 5 Bytes maximum length for transmit and receive 0 and 1 (LSByte is written first. Write the number of bytes defined by SETUP_AW)
    ///   Receive 2 - 5 are only 1 Byte
    ///   For Transmit address set RX_ADDR_P0 equal to this address to handle automatic acknowledge if this is a PTX device with Enhanced ShockBurst™ enabled.
    /// </summary>
    public class AddressPipeRegister : PipeRegisterBase
    {
        public AddressPipeRegister(ICommandProcessor commandProcessor, byte address, byte pipeNumber) :
            base(commandProcessor, address, (byte)(pipeNumber <= 1 ? 5 : 1), pipeNumber)
        { }
    }
}
