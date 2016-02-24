using System;
using System.Diagnostics;
using System.Runtime.InteropServices.WindowsRuntime;
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

            _radio = new Radio(commandProcessor, powerPin, cePin, irqPin);

            //ConstantCarrierWaveOutputForTesting();
            //SimpleSendTest();
            //SimpleReceiveTest();
            SimpleSendReceiveTest();
        }

        public void Dispose()
        {
            _radio?.Dispose();
            _radio = null;
        }

        public byte[] GetBytes()
        {
            throw new NotImplementedException();
        }

        public void WriteBytes([ReadOnlyArray] byte[] bytes)
        {
            throw new NotImplementedException();
        }

        public void SimpleReceiveTest()
        {
            SimpleReceiverRole receiver = new SimpleReceiverRole();
            receiver.AttachRadio(_radio);
            receiver.ReceiveAddress = _receiveAddress;
            receiver.DataArrived += DataArrived; ;
            receiver.Start();
            PrintDetails();
            while (true) { }
        }

        public void SimpleSendTest()
        {
            SimpleSendRole sender = new SimpleSendRole();
            sender.AttachRadio(_radio);
            sender.SendAddress = _sendAddress;
            sender.Start();
            int count = 0;
            PrintDetails();
            while (true)
            {
                lock (_syncRoot)
                {
                    byte[] payload = Encoding.UTF8.GetBytes("Hello World " + (count++)).ReverseBytes();
                    Debug.WriteLine(sender.Send(payload) ? "Send complete" : "Send failed");
                }
                Task.Delay(1000).Wait();
            }
        }

        public void SimpleSendReceiveTest()
        {
            SendReceiveRole sendReceive = new SendReceiveRole();
            sendReceive.AttachRadio(_radio);
            sendReceive.DataArrived += DataArrived; ;
            sendReceive.SendAddress = _sendAddress;
            sendReceive.ReceiveAddress = _receiveAddress;
            sendReceive.Start();
            int count = 0;
            PrintDetails();
            while (true)
            {
                string content = "Payload, Count=" + (count++).ToString();
                lock (_syncRoot)
                {
                    Debug.WriteLine(sendReceive.Send(Encoding.UTF8.GetBytes(content).ReverseBytes(), 5000)
                        ? "Data sent success."
                        : "Failed to send data.");
                }

                Task.Delay(1000).Wait();
            }
        }

        private void DataArrived(object sender, byte[] data)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte value in data)
                sb.AppendFormat("0x{0} ", value.ToString("X").PadLeft(2, '0'));
            Debug.WriteLine("Data Received, Data =" + sb);
        }

        private void PrintDetails()
        {
            // Debug.WriteLine(_radio.GetArduinoDetails());
        }

        private void PrintDiagnostics()
        {
            // Debug.WriteLine(_radio.ToString());
        }
    }
}
