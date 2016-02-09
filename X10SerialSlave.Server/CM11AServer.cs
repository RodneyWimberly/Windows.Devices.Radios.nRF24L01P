using HA4IoT.Hardware.X10;
//using HA4IoT.Hardware.X10.Codes;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.SerialCommunication;
using Windows.Storage.Streams;

namespace X10SerialSlave.Server
{
    /// <summary>
    /// Commands that may be sent to an X10 Controller.
    /// </summary>
    public enum X10Command
    {
        AllUnitsOff = 0,
        AllLightsOn = 1,
        TurnOn = 2,
        TurnOff = 3,
        Dim = 4,
        Brighten = 5,
        AllLightsOff = 6
    }
    /// <summary>
    /// Enables control of the X10 CM11A device.
    /// </summary>
    public sealed class CM11AServer : IX10Controller, IDisposable
    {
        #region Static members
        static readonly ConcurrentDictionary<string, CM11AServer> Instances;

        const byte _deviceReadyCode = 0x55;
        const byte _acknowledgeCode = 0x00;
        const byte _addressNotificationCode = 0x04;
        const byte _functionNotificationBaseCode = 0x06;
        const byte _powerFailureCode = 0xA5;
        const byte _pollSignalCode = 0x5A;
        const byte _pollAcknowledgementCode = 0xC3;
        readonly byte[] _addressCodes = { 0x06, 0x0E, 0x02, 0x0A, 0x01, 0x09, 0x05, 0x0D, 0x07, 0x0F, 0x03, 0x0B, 0x00, 0x08, 0x04, 0x0C };
        readonly byte[] _functionCodes = { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06 };

        static CM11AServer()
        {
            Instances = new ConcurrentDictionary<string, CM11AServer>();
        }

        public static CM11AServer GetInstance()
        {
            string aqs = SerialDevice.GetDeviceSelector("UART0");
            DeviceInformationCollection dic = DeviceInformation.FindAllAsync(aqs).GetAwaiter().GetResult();
            DeviceInformation di = dic.FirstOrDefault();
            return GetInstance(di.Id);
        }

        /// <summary>
        /// Gets an instance the CM11A device and opens the specified port for communication.
        /// </summary>
        /// <param name="portName">The name of the port to open, such as COM1.</param>
        public static CM11AServer GetInstance(string portName)
        {
            if (string.IsNullOrEmpty(portName))
                throw new ArgumentNullException(nameof(portName));

            CM11AServer instance = null;
            lock (Instances)
            {
                if (Instances.ContainsKey(portName))
                {
                    instance = Instances[portName];
                }
                else
                {
                    try
                    {
                        instance = new CM11AServer(portName);
                    }
                    catch (Exception ex)
                    {
                        instance = null;
                        throw new IOException(string.Format("There was an error creating the CM11A device on {0}.", portName), ex);
                    }
                    finally
                    {
                        if (instance != null)
                        {
                            Interlocked.Increment(ref instance._refCount);
                            Instances.AddOrUpdate(portName, instance, (key, existingVal) => instance);
                        }
                    }
                }
            }

            return instance;
        }
        #endregion

        #region Instance members
        private readonly char _houseCodeToMonitor;
        private readonly CancellationTokenSource _readCancellationTokenSource;
        private SerialDevice _serialDevice;
        private DataWriter _dataWriteObject;
        private DataReader _dataReaderObject;
        private int _refCount;
        private readonly object _syncRoot;

        public string PortName { get; private set; }

        /// <summary>
        /// Creates the CM11A device and opens the specified port for communication.
        /// </summary>
        /// <param name="portName">The name of the port to open, such as "COM1".</param>
        private CM11AServer(string portName)
        {
            _syncRoot = new object();
            _refCount = 0;
            _houseCodeToMonitor = 'A';
            _readCancellationTokenSource = new CancellationTokenSource();

            PortName = portName;
        }

        public void Initialize()
        {
            try
            {
                _serialDevice = SerialDevice.FromIdAsync(PortName).GetAwaiter().GetResult();
                _serialDevice.WriteTimeout = TimeSpan.FromMilliseconds(2500);
                _serialDevice.ReadTimeout = TimeSpan.FromMilliseconds(2500);
                _serialDevice.BaudRate = 9600;
                _serialDevice.StopBits = SerialStopBitCount.One;
                _serialDevice.DataBits = 8;
                _serialDevice.Parity = SerialParity.None;
                _serialDevice.Handshake = SerialHandshake.None;
                _serialDevice.PinChanged += _serialDevice_PinChanged;
            }
            catch (Exception ex)
            {
                Dispose();
                throw new IOException(string.Format("There was an error creating the CM11A device on {0}.", PortName), ex);
            }
        }

        private void _serialDevice_PinChanged(SerialDevice sender, PinChangedEventArgs args)
        {
            if (args.PinChange != SerialPinChange.RingIndicator) return;
            _dataReaderObject = new DataReader(_serialDevice.InputStream)
            {
                InputStreamOptions = InputStreamOptions.Partial
            };
            byte response = _dataReaderObject.ReadByte();
            _dataReaderObject.DetachStream();
            _dataReaderObject = null;

            HandleResponse(response);
        }

        /// <summary>
        /// Sends a command to the CM11A device for a specific house code and unit code.
        /// </summary>
        /// <param name="houseCode">The house code of the unit to control.</param>
        /// <param name="unitCode">The number (1-16) of the unit to control.</param>
        /// <param name="command">The command to send to the specified unit.</param>
        public void SendCommand(char houseCode, int unitCode, X10Command command)
        {
            SendCommand(houseCode, unitCode, command, Constants.DefaultDimBrightAmount);
        }

        /// <summary>
        /// Sends a command to the CM11A device for a specific house code and unit code.
        /// </summary>
        /// <param name="houseCode">The house code of the unit to control.</param>
        /// <param name="unitCode">The number (1-16) of the unit to control.</param>
        /// <param name="command">The command to send to the specified unit.</param>
        /// <param name="dimOrBrightAmount">The percentage (1-100) the specified unit will be dimmed or brightened.</param>
        public void SendCommand(char houseCode, int unitCode, X10Command command, int dimOrBrightAmount)
        {
            if (unitCode < 1 || unitCode > 16)
                throw new ArgumentOutOfRangeException(nameof(unitCode), unitCode, "The unit code must be between 1 - 16.");

            if (dimOrBrightAmount < 1 || dimOrBrightAmount > 100)
                throw new ArgumentOutOfRangeException(nameof(dimOrBrightAmount), dimOrBrightAmount, "dimOrBrightAmount must be between 1 - 100");

            byte[] address = CreateAddress(houseCode, unitCode);
            byte[] function = CreateFunction(houseCode, command, dimOrBrightAmount);

            bool success = false;
            int numAttempts = 0;

            lock (_syncRoot)
            {
                do
                {
                    numAttempts++;
                    if (numAttempts > 10)
                        throw new IOException("The CM11A device was unable to send the requested address.");
                    try
                    {
                        if (SendCommandBytes(address))
                            success = SendCommandBytes(function);
                    }
                    catch { }

                } while (!success);
            }
        }

        /// <summary>
        /// Disposes of the CM11A object and closes the serial port.
        /// </summary>
        public void Dispose()
        {
            if (Interlocked.Decrement(ref _refCount) != 0) return;
            lock (Instances)
            {
                CM11AServer cm11A;
                Instances.TryRemove(PortName, out cm11A);
            }
            _serialDevice?.Dispose();
            _serialDevice = null;
        }

        private bool SendCommandBytes(byte[] command)
        {
            byte checksum = CreateChecksum(command);
            WriteBytes(command);
            byte[] response = GetBytes();
            bool success = (checksum == response[0]);
            if (!success)
                HandleResponse(response[0]);
            else
            {
                WriteBytes(new byte[] { _acknowledgeCode });
                GetBytes();
            }

            return success;
        }

        private byte CreateChecksum(byte[] command)
        {
            return Convert.ToByte((command[0] + command[1]) & 0xff);
        }

        private byte GetHouseCodeByte(char houseCode)
        {
            int code = (int)houseCode;
            // Is it a lower case a - p? If so remove 32 so it will be upper case
            if (code > 96 && code < 113) code -= 32;
            if (code < 65 || code > 80)
                throw new ArgumentOutOfRangeException(nameof(houseCode), houseCode, "Invalid House Code, please use a valid House Code in the range of A - P");
            return _addressCodes[code - 65];
        }

        private byte GetUnitCodeByte(int unitCode)
        {
            if (unitCode < 1 || unitCode > 16)
                throw new ArgumentOutOfRangeException(nameof(unitCode), unitCode, "Invalid Unit Code, please use a valid Unit Code in the range of 1 - 16");
            return _addressCodes[unitCode - 1];
        }

        private byte GetCommandCodeByte(X10Command command)
        {
            return _functionCodes[(int)command];
        }

        private byte[] CreateAddress(char houseCode, int unitCode)
        {
            // Assemble the address (4 bit house code then 4 bit unit code) of the device we're addressing
            byte houseCodeByte = GetHouseCodeByte(houseCode);
            byte unitCodeByte = GetUnitCodeByte(unitCode);
            byte address = Convert.ToByte((houseCodeByte << 4) | unitCodeByte); // Move the house code to the left by four bits and append the unit code as the right four bits.

            return new[] { _addressNotificationCode, address };
        }

        private byte[] CreateFunction(char houseCode, X10Command command, int dimOrBrightAmount)
        {
            // Assemble the function (4 bit house code then 4 bit function code)
            byte houseCodeByte = GetHouseCodeByte(houseCode);
            byte functionCodeByte = GetCommandCodeByte(command);
            byte function = Convert.ToByte((houseCodeByte << 4) | functionCodeByte); // Move the house code to the left by four bits and append the function code as the right four bits.
            byte functionNotification = new byte();

            if (command == X10Command.Brighten || command == X10Command.Dim)
            {
                // Normalize the dim/brighten amount to send. The values are integers between 0 and 22.
                byte normalizedDimBrighten = Convert.ToByte(Math.Floor(dimOrBrightAmount * .01 * 22));
                functionNotification = Convert.ToByte((normalizedDimBrighten << 3) | _functionNotificationBaseCode); // The dim amount is the left five bits and the functionNotificationBase is the right three bits.
            }
            else
            {
                functionNotification = _functionNotificationBaseCode; // We don't send a dim/brighten amount, just the function notification bits.
            }

            // Calculate the checksum for this function
            byte checksum = Convert.ToByte((functionNotification + function) & 0xff);

            return new[] { functionNotification, function };
        }

        private void HandleResponse(byte response)
        {
            if (response == _powerFailureCode)
                SetClock();
            else if (response == _pollSignalCode)
                HandleIncomingData();
        }

        private void AcknowledgePoll()
        {
            WriteBytes(new[] { _pollAcknowledgementCode });
        }

        private void HandleIncomingData()
        {
            lock (_syncRoot)
            {
                byte[] rawDataLength;

                do
                {
                    AcknowledgePoll();
                    rawDataLength = GetBytes();
                } while (rawDataLength[0] == _pollSignalCode);

                byte dataLength = (byte)(rawDataLength[0] - 1);
                GetBytes();
                List<byte> dataBuffer = new List<byte>();
                for (int i = 0; i <= dataLength - 1; i++)
                    dataBuffer.Add(GetBytes()[0]);
            }
        }

        private void SetClock()
        {
            DateTime now = DateTime.Now;
            byte[] clockCommand = new byte[7];
            clockCommand[0] = 0x9b;
            clockCommand[1] = (byte)now.Second;
            int totalMinutes = (now.Hour * 60) + now.Minute;
            clockCommand[2] = (byte)(totalMinutes % 120);
            clockCommand[3] = (byte)(now.Hour / 2);
            clockCommand[4] = (byte)(now.DayOfYear & 0xff);
            clockCommand[5] = (byte)(2 ^ (int)now.DayOfWeek);
            if (now.DayOfYear > 255)
                clockCommand[5] = (byte)(clockCommand[5] | 0x80);
            clockCommand[6] = (byte)(GetHouseCodeByte(_houseCodeToMonitor) << 4);

            WriteBytes(clockCommand);
            GetBytes();

            WriteBytes(new[] { _acknowledgeCode });
            GetBytes();
        }

        public byte[] GetBytes()
        {
            byte[] data = null;

            try
            {
                if (_serialDevice != null)
                {
                    CancellationToken token = _readCancellationTokenSource.Token;
                    token.ThrowIfCancellationRequested();

                    _dataReaderObject = new DataReader(_serialDevice.InputStream) { InputStreamOptions = InputStreamOptions.Partial };
                    Task<UInt32> loadAsyncTask = _dataReaderObject.LoadAsync(1024).AsTask(token);

                    // Launch the task and wait
                    if (loadAsyncTask.Wait(50009))
                    {
                        //UInt32 bytesRead = loadAsyncTask.Result;
                        //uint bytesRead = _dataReaderObject.LoadAsync(1024).AsTask(token).Result;
                        if (loadAsyncTask.Result > 0)
                            _dataReaderObject.ReadBytes(data);
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex.GetType().Name != "TaskCanceledException")
                    throw;
            }
            finally
            {
                _dataReaderObject?.DetachStream();
                _dataReaderObject = null;
            }

            return data;
        }
        #endregion

        public void WriteBytes([ReadOnlyArray] byte[] bytes)
        {
            try
            {
                if (_serialDevice != null)
                {
                    CancellationToken token = _readCancellationTokenSource.Token;
                    token.ThrowIfCancellationRequested();

                    _dataWriteObject = new DataWriter(_serialDevice.OutputStream);
                    if (bytes.Length != 0)
                    {
                        _dataWriteObject.WriteBytes(bytes);
                        _dataWriteObject.StoreAsync().AsTask(token).Wait(token);
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex.GetType().Name != "TaskCanceledException")
                    throw;
            }
            finally
            {
                _dataWriteObject?.DetachStream();
                _dataWriteObject = null;
            }
        }
    }
}
