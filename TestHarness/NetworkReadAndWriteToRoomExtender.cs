using Common.Logging;
using Common.Logging.WinRT.Extras;
using System;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.Gpio;
using Windows.Devices.Radios.nRF24L01P;
using Windows.Devices.Radios.nRF24L01P.Enums;
using Windows.Devices.Radios.nRF24L01P.Extensions;
using Windows.Devices.Radios.nRF24L01P.Interfaces;
using Windows.Devices.Radios.nRF24L01P.Network;
using Windows.Devices.Radios.nRF24L01P.Network.Interfaces;
using Windows.Devices.Spi;
using Windows.System.Threading;

namespace nRF24L01P.TestHarness
{
    internal class NetworkReadAndWriteToRoomExtender : IReadAndWriteProcess
    {
        private readonly Radio _radio;
        private readonly Network _network;
        private readonly ILog _logger;
        private readonly TaskFactory _taskFactory;
        private int _counter;

        public NetworkReadAndWriteToRoomExtender()
        {
            _counter = 0;
            ILoggerFactoryAdapter loggerFactory = new DebugOutLoggerFactoryAdapter(LogLevel.All, true, true, true, "MM/dd/yyyy hh:mm:ss");
            _logger = loggerFactory.GetLogger(GetType());
            _taskFactory = new TaskFactory(TaskCreationOptions.LongRunning, TaskContinuationOptions.ExecuteSynchronously);

            GpioController gpioController = GpioController.GetDefault();
            GpioPin powerPin = gpioController.InitGpioPin(4, GpioPinDriveMode.Output, GpioSharingMode.Exclusive);//4
            GpioPin cePin = gpioController.InitGpioPin(22, GpioPinDriveMode.Output, GpioSharingMode.Exclusive);//2
            DeviceInformationCollection devicesInfo = DeviceInformation.FindAllAsync(SpiDevice.GetDeviceSelector("SPI0")).GetAwaiter().GetResult();
            SpiDevice spiDevice = SpiDevice.FromIdAsync(devicesInfo[0].Id, new SpiConnectionSettings(0)).GetAwaiter().GetResult();
            ICommandProcessor commandProcessor = new CommandProcessor(spiDevice);

            powerPin.Write(GpioPinValue.Low);
            cePin.Write(GpioPinValue.Low);

            _radio = new Radio(commandProcessor, loggerFactory, powerPin, cePin);
            _radio.Configuration.PowerLevel = PowerLevels.High;
            _radio.Configuration.DataRate = DataRates.DataRate250Kbps;
            _network = new Network(loggerFactory, _radio);
            _network.Begin(100, 00);
        }

        public void Dispose()
        {
            _radio.Dispose();
        }

        public void Initialize()
        {
            ThreadPoolTimer.CreatePeriodicTimer(Handler, TimeSpan.FromSeconds(1));
        }

        private async void Handler(ThreadPoolTimer timer)
        {
            _counter++;
            timer.Cancel();
            _network.Update();
            if (_counter > 5)
            {
                _counter = 0;
                await _taskFactory.StartNew(() => SendX10Packet('M', 1, 3));
            }
            ThreadPoolTimer.CreatePeriodicTimer(Handler, TimeSpan.FromSeconds(1));
        }

        public bool SendX10Packet(char house, byte unit, byte command)
        {
            bool success = false;
            byte[] packet = new byte[] { (byte)house, unit, command, 0, 0, 0, 0 }.ReverseBytes();

            success = _network.Write(CreateHeader(HeaderTypes.reX10Standard), packet, packet.Length);
            _logger.Debug("SendX10Packet " + (success ? "succeeded!" : "failed!") + " { " + packet[0] + ", " + packet[1] + ", " + packet[2] + " }");

            return success;
        }

        public bool SendEnvironmentRequestPacket()
        {
            bool success = false;
            byte[] packet = { };
            success = _network.Write(CreateHeader(HeaderTypes.reEnvironmentSensor), packet, packet.Length);
            _logger.Debug("SendEnvironmentRequestPacket " + (success ? "succeeded!" : "failed!"));
            return success;
        }

        private INetworkHeader CreateHeader(HeaderTypes type)
        {
            return new NetworkHeader
            {
                FromNode = _network.Addressing.NodeAddress,
                ToNode = 01,
                Type = (byte)type
            };
        }
    }
}
