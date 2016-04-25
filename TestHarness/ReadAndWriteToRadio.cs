using Common.Logging;
using Common.Logging.WinRT.Extras;
using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.Gpio;
using Windows.Devices.Radios.nRF24L01P;
using Windows.Devices.Radios.nRF24L01P.Extensions;
using Windows.Devices.Radios.nRF24L01P.Interfaces;
using Windows.Devices.Radios.nRF24L01P.Roles;
using Windows.Devices.Spi;

namespace nRF24L01P.TestHarness
{
    public sealed class ReadAndWriteToRadio : IReadAndWriteProcess
    {
        private Radio _radio;
        private readonly byte[] _sendAddress;
        private readonly byte[] _receiveAddress;
        private readonly object _syncRoot;

        public ReadAndWriteToRadio()
        {
            _syncRoot = new object();
            _sendAddress = new byte[] { 0x54, 0x4d, 0x52, 0x68, 0x7C };
            _receiveAddress = new byte[] { 0xAB, 0xCD, 0xAB, 0xCD, 0x71 };
        }

        public void Initialize()
        {
            GpioController gpioController = GpioController.GetDefault();
            GpioPin powerPin = gpioController.InitGpioPin(4, GpioPinDriveMode.Output, GpioSharingMode.Exclusive);
            GpioPin cePin = gpioController.InitGpioPin(22, GpioPinDriveMode.Output, GpioSharingMode.Exclusive);
            GpioPin irqPin = gpioController.InitGpioPin(25, GpioPinDriveMode.InputPullUp, GpioSharingMode.Exclusive);
            powerPin.Write(GpioPinValue.Low);
            cePin.Write(GpioPinValue.Low);
            irqPin.Write(GpioPinValue.High);

            DeviceInformationCollection devicesInfo = DeviceInformation.FindAllAsync(SpiDevice.GetDeviceSelector("SPI0")).GetAwaiter().GetResult();
            SpiDevice spiDevice = SpiDevice.FromIdAsync(devicesInfo[0].Id, new SpiConnectionSettings(0)).GetAwaiter().GetResult();
            ICommandProcessor commandProcessor = new CommandProcessor(spiDevice);

            ILoggerFactoryAdapter loggerFactoryAdapter = new DebugOutLoggerFactoryAdapter(LogLevel.All, true, true, true, "MM/dd/yyyy hh:mm:ss fffff");

            _radio = new Radio(commandProcessor, loggerFactoryAdapter, powerPin, cePin, irqPin);

            //SenderTest();
            //ReceiverTest();
            SenderReceiverTest();
        }

        public void ReceiverTest()
        {
            ReceiverRole receiver = new ReceiverRole();
            receiver.AttachRadio(_radio);
            receiver.ReceiveAddress = _receiveAddress;
            receiver.DataArrived += DataArrived; ;
            receiver.Start();
            while (true) { }
        }

        public void SenderTest()
        {
            SenderRole sender = new SenderRole();
            sender.AttachRadio(_radio);
            sender.SendAddress = _sendAddress;
            sender.Start();
            int count = 0;
            while (true)
            {
                string content = "Payload, Count=" + (count++);
                byte[] buffer = new byte[3];
                buffer[0] = 6;
                buffer[1] = 12;
                buffer[2] = 5;
                lock (_syncRoot)
                {
                    //Encoding.UTF8.GetBytes(content).ReverseBytes()
                    Debug.WriteLine(sender.Send(buffer.ReverseBytes())
                        ? "Send complete"
                        : "Send failed " + (sender.MaxRetries ? "MaxRetries" : "Timeout"));
                }
                Task.Delay(1000).Wait();
            }
        }

        public void SenderReceiverTest()
        {
            SenderReceiverRole senderReceiver = new SenderReceiverRole();
            senderReceiver.AttachRadio(_radio);
            senderReceiver.DataArrived += DataArrived; ;
            senderReceiver.SendAddress = _sendAddress;
            senderReceiver.ReceiveAddress = _receiveAddress;
            senderReceiver.Start();
            int count = 0;
            while (true)
            {
                string content = "Payload, Count=" + (count++);
                lock (_syncRoot)
                {
                    Debug.WriteLine(senderReceiver.Send(Encoding.UTF8.GetBytes(content).ReverseBytes(), 5000)
                        ? "Data sent success."
                        : "Failed to send data. " + (senderReceiver.MaxRetries ? "MaxRetries" : "Timeout"));
                }
                Task.Delay(1000).Wait();
            }
        }

        private void DataArrived(object sender, byte[] data)
        {
            string content = Encoding.UTF8.GetString(data, 0, data.Length - 1);
            Debug.WriteLine("Data Received, Data = " + content);
        }

        public void Dispose()
        {
            _radio?.Dispose();
            _radio = null;
        }

    }
}
