using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System;
using System.Diagnostics;
using Windows.Devices.Enumeration;
using Windows.Devices.Gpio;
using Windows.Devices.Radios.nRF24L01P.Registers;
using Windows.Devices.Spi;

namespace Windows.Devices.Radios.nRF24L01P.IntegrationTests
{
    [TestClass]
    public class AddressPipeRegisterTests
    {
        private Radio _radio;
        private long[] _pipes = new[] { 0xF0F0F0F0E1, 0xF0F0F0F0D2 };

        [TestInitialize]
        public void TestInitialize()
        {
            GpioPin cePin = GpioController.GetDefault().OpenPin(22);

            DeviceInformationCollection devicesInfo = DeviceInformation.FindAllAsync(SpiDevice.GetDeviceSelector("SPI0")).GetAwaiter().GetResult();
            SpiDevice spiDevice = SpiDevice.FromIdAsync(devicesInfo[0].Id, new SpiConnectionSettings(0)).GetAwaiter().GetResult();

            _radio = new Radio(cePin, spiDevice);
            _radio.Begin();
            _radio.OpenReadingPipe(1, _pipes[0]);
            _radio.StartListening();
            Debug.WriteLine(_radio.GetDetails());
        }

        [TestMethod]
        public void TestWriteAndRead()
        {
            byte[] buffer = BitConverter.GetBytes(_pipes[1]);
            PrintBytes(buffer);
            Array.Resize(ref buffer, 5);
            PrintBytes(buffer);
            AddressPipeRegister receivePipe = _radio.Configuration.Registers.ReceiveAddressPipeRegisters[1];
            receivePipe.Load(buffer);
            PrintBytes(receivePipe);
            receivePipe.Save();
            PrintBytes(receivePipe);
            receivePipe.Load();
            PrintBytes(receivePipe);
        }

        private void PrintBytes(byte[] bytes)
        {
            Debug.WriteLine(BitConverter.ToString(bytes).Replace("-", string.Empty));
        }
    }
}
