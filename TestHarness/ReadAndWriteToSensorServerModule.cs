using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.Gpio;
using Windows.Devices.Radios.nRF24L01P;
using Windows.Devices.Radios.nRF24L01P.Extensions;
using Windows.Devices.Radios.nRF24L01P.Interfaces;
using Windows.Devices.Radios.nRF24L01P.Roles;
using Windows.Devices.Spi;
using Windows.System.Threading;

namespace nRF24L01P.TestHarness
{
    internal class ReadAndWriteToSensorServerModule : IReadAndWriteProcess
    {
        private Radio _radio;
        private readonly byte[] _sendAddress;
        private readonly byte[] _receiveAddress;
        private SenderReceiverRole _sendReceiveRole;
        private readonly ManualResetEvent _manualResetEvent;
        private readonly object _syncRoot;

        public ReadAndWriteToSensorServerModule()
        {
            _sendAddress = new byte[] { 0x54, 0x4d, 0x52, 0x68, 0x7C };
            _receiveAddress = new byte[] { 0xAB, 0xCD, 0xAB, 0xCD, 0x71 };
            _manualResetEvent = new ManualResetEvent(false);
            _syncRoot = new object();
        }

        public void Dispose()
        {
            _radio.Dispose();
            _radio = null;
        }

        public void Initialize()
        {
            GpioController gpioController = GpioController.GetDefault();
            GpioPin powerPin = gpioController.InitGpioPin(4, GpioPinDriveMode.Output, GpioSharingMode.Exclusive);//4
            GpioPin cePin = gpioController.InitGpioPin(22, GpioPinDriveMode.Output, GpioSharingMode.Exclusive);//22
            GpioPin irqPin = gpioController.InitGpioPin(25, GpioPinDriveMode.InputPullUp, GpioSharingMode.Exclusive);//25
            powerPin.Write(GpioPinValue.Low);
            cePin.Write(GpioPinValue.Low);
            irqPin.Write(GpioPinValue.High);

            DeviceInformationCollection devicesInfo = DeviceInformation.FindAllAsync(SpiDevice.GetDeviceSelector("SPI0")).GetAwaiter().GetResult();
            SpiDevice spiDevice = SpiDevice.FromIdAsync(devicesInfo[0].Id, new SpiConnectionSettings(0)).GetAwaiter().GetResult();
            ICommandProcessor commandProcessor = new CommandProcessor(spiDevice);

            _radio = new Radio(commandProcessor, powerPin, cePin, irqPin);
            _sendReceiveRole = new SenderReceiverRole();
            _sendReceiveRole.AttachRadio(_radio);
            _sendReceiveRole.DataArrived += DataArrived; ;
            _sendReceiveRole.SendAddress = _sendAddress;
            _sendReceiveRole.ReceiveAddress = _receiveAddress;
            _sendReceiveRole.Start();

            ThreadPoolTimer.CreatePeriodicTimer(Handler, TimeSpan.FromSeconds(5));
            WaitForData();
        }

        private async void Handler(ThreadPoolTimer timer)
        {
            timer.Cancel();
            Debug.WriteLine("TimerCallback");
            TaskFactory f = new TaskFactory(TaskCreationOptions.LongRunning, TaskContinuationOptions.ExecuteSynchronously);
            bool success = await f.StartNew(() => SendX10Packet('A', 1, 3));
            ThreadPoolTimer.CreatePeriodicTimer(Handler, TimeSpan.FromSeconds(5));
        }

        public bool SendX10Packet(char house, int unit, byte command)
        {
            int count = 0;
            lock (_syncRoot)
            {
                byte[] packet = { (byte)wrPacketTypes.X10StandardRequest, (byte)house, (byte)unit, command };

                while (!_sendReceiveRole.Send(packet.ReverseBytes()))
                {
                    count++;
                    Task.Delay(250).Wait();
                    if (count > 20)
                        break;
                }
                Debug.WriteLine("SendX10Packet " + (count <= 20 ? "succeeded!" : "failed!"));
            }

            return count <= 20;
        }

        public bool SendEnvironmentRequestPacket()
        {
            int count = 0;
            lock (_syncRoot)
            {
                byte[] packet = { (byte)wrPacketTypes.EnvironmentSensorRequest };
                while (!_sendReceiveRole.Send(packet.ReverseBytes()))
                {
                    count++;
                    Task.Delay(250).Wait();
                    if (count > 10)
                        break;
                }
                Debug.WriteLine("SendEnvironmentRequestPacket " + (count <= 10 ? "succeeded!" : "failed!"));
            }
            return count <= 10;
        }

        private void DataArrived(object sender, byte[] e)
        {
            string content = Encoding.UTF8.GetString(e, 0, e.Length - 1);
            Debug.WriteLine("Data Received, Data = " + content);
        }

        private void WaitForData()
        {
            _manualResetEvent.Reset();
            while (true)
            {
                lock (_syncRoot)
                {
                    Debug.WriteLine("WaitForData");
                    if (!_manualResetEvent.WaitOne(1000))
                        continue;
                    break;
                }
            }
        }

    }
}
