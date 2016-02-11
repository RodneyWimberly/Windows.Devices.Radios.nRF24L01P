namespace Windows.Devices.Radios.nRF24L01P.Interfaces
{

    public interface IReceivePipe : IDataPipe
    {
        int PipeId { get; }
        bool Enabled { get; set; }
        bool AutoAcknowledgementEnabled { get; set; }
        bool DynamicPayloadLengthEnabled { get; set; }
        byte PayloadWidth { get; set; }
        byte BytesToRead { get; }

        /// <summary>
        /// Reads data from RX buffer, use this feature when dynamic payload length is turned off
        /// Warning: use this feature will ignore the data pipe ID, it may read buffer belongs to other data pipe
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        byte[] ReadBuffer(int length);

        /// <summary>
        /// Reads data from RX buffer, use Dynamic payload length feature to detect package length
        /// </summary>
        /// <returns></returns>
        byte[] ReadBufferAll(int readBufferSize = 512);

        /// <summary>
        /// reads all data available in FIFO buffer to a byte array
        /// </summary>
        /// <param name="outputBuffer">output buffer</param>
        /// <param name="writeStartPosition">start position in output buffer</param>
        /// <returns></returns>
        int ReadBufferAll(byte[] outputBuffer, int writeStartPosition);
    }
}