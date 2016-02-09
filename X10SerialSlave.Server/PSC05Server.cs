using System;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.SerialCommunication;
using Windows.Storage.Streams;

namespace X10SerialSlave.Server
{
    public sealed class PSC05Server : IX10Controller
    {

        private SerialDevice _serialPort = null;
        private DataWriter _dataWriteObject = null;
        private DataReader _dataReaderObject = null;

        private CancellationTokenSource _readCancellationTokenSource;
        private StringBuilder _readBuffer;

        public void Initialize()
        {
            _readBuffer = new StringBuilder();
            InitializeComPort();
        }

        private async void InitializeComPort()
        {
            string aqs = SerialDevice.GetDeviceSelector();
            DeviceInformationCollection dic = await DeviceInformation.FindAllAsync(aqs).AsTask();
            DeviceInformation di = dic.FirstOrDefault();

            _serialPort = await SerialDevice.FromIdAsync(di.Id).AsTask();
            _serialPort.IsDataTerminalReadyEnabled = true;
            _serialPort.BaudRate = 9600;

            _readCancellationTokenSource = new CancellationTokenSource();
        }

        public async void Send(string data)
        {
            UInt32 bytesWritten = 0;
            try
            {
                if (_serialPort != null)
                {
                    // Create the DataWriter object and attach to OutputStream
                    _dataWriteObject = new DataWriter(_serialPort.OutputStream);

                    //Launch the WriteAsync task to perform the write
                    byte[] bytes = Encoding.ASCII.GetBytes(data);
                    bytesWritten = await WriteAsync(bytes);
                }
            }
            catch (Exception)
            {
            }
            finally
            {
                // Cleanup once complete
                if (_dataWriteObject != null)
                {
                    _dataWriteObject.DetachStream();
                    _dataWriteObject = null;
                }
            }
        }

        /// <summary>
        /// WriteAsync: Task that asynchronously writes data from the input text box 'sendText' to the OutputStream 
        /// </summary>
        /// <returns></returns>
        private async Task<UInt32> WriteAsync(byte[] data)
        {
            UInt32 bytesWritten = 0;
            if (data.Length != 0)
            {
                // Load the text from the sendText input text box to the dataWriter object
                _dataWriteObject.WriteBytes(data);

                // Launch an async task to complete the write operation
                bytesWritten = await _dataWriteObject.StoreAsync().AsTask();
            }
            return bytesWritten;
        }

        /// <summary>
        /// - Create a DataReader object
        /// - Create an async task to read from the SerialDevice InputStream
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Listen()
        {
            try
            {
                if (_serialPort != null)
                {
                    _dataReaderObject = new DataReader(_serialPort.InputStream);

                    // keep reading the serial input
                    while (true)
                    {
                        await ReadAsync(_readCancellationTokenSource.Token);
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex.GetType().Name == "TaskCanceledException")
                {
                    CloseDevice();
                }
            }
            finally
            {
                // Cleanup once complete
                if (_dataReaderObject != null)
                {
                    _dataReaderObject.DetachStream();
                    _dataReaderObject = null;
                }
            }
        }

        /// <summary>
        /// ReadAsync: Task that waits on data and reads asynchronously from the serial device InputStream
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task ReadAsync(CancellationToken cancellationToken)
        {
            string data = string.Empty;
            uint ReadBufferLength = 1024;

            // If task cancellation was requested, comply
            cancellationToken.ThrowIfCancellationRequested();

            // Set InputStreamOptions to complete the asynchronous read operation when one or more bytes is available
            _dataReaderObject.InputStreamOptions = InputStreamOptions.Partial;

            // Create a task object to wait for data on the serialPort.InputStream
            Task<UInt32> loadAsyncTask = _dataReaderObject.LoadAsync(ReadBufferLength).AsTask(cancellationToken);

            // Launch the task and wait
            UInt32 bytesRead = await loadAsyncTask;
            if (bytesRead > 0) data = _dataReaderObject.ReadString(bytesRead);
            _readBuffer.Append(data);
        }

        /// <summary>
        /// CancelReadTask:
        /// - Uses the ReadCancellationTokenSource to cancel read operations
        /// </summary>
        public void CancelReadTask()
        {
            if (_readCancellationTokenSource != null)
            {
                if (!_readCancellationTokenSource.IsCancellationRequested)
                {
                    _readCancellationTokenSource.Cancel();
                }
            }
        }

        /// <summary>
        /// CloseDevice:
        /// - Disposes SerialDevice object
        /// - Clears the enumerated device Id list
        /// </summary>
        private void CloseDevice()
        {
            if (_serialPort != null)
            {
                _serialPort.Dispose();
            }
            _serialPort = null;

        }

        /// <summary>
        /// closeDevice_Click: Action to take when 'Disconnect and Refresh List' is clicked on
        /// - Cancel all read operations
        /// - Close and dispose the SerialDevice object
        /// - Enumerate connected devices
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Close()
        {
            CancelReadTask();
            CloseDevice();
        }

        public byte[] GetBytes()
        {
            return Encoding.ASCII.GetBytes(_readBuffer.ToString());
        }

        public async void WriteBytes([ReadOnlyArray] byte[] bytes)
        {
            try
            {
                if (_serialPort != null)
                {
                    // Create the DataWriter object and attach to OutputStream
                    _dataWriteObject = new DataWriter(_serialPort.OutputStream);
                    await WriteAsync(bytes);
                }
            }
            catch (Exception)
            {
            }
            finally
            {
                _dataWriteObject?.DetachStream();
                _dataWriteObject = null;
            }
        }
    }
}
