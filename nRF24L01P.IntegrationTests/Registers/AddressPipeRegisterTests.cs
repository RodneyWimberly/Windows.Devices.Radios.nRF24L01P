using System;
using System.Diagnostics;
using Windows.Devices.Enumeration;
using Windows.Devices.Gpio;
using Windows.Devices.Radios.nRF24L01P.Registers;
using Windows.Devices.Spi;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace Windows.Devices.Radios.nRF24L01P.IntegrationTests.Registers
{
    [TestClass]
    public class AddressPipeRegisterTests
    {
        private Radio _radio;
        private long[] _pipes = new[] { 0xF0F0F0F0E1, 0xF0F0F0F0D2 };

        [ClassInitialize]
        public async void ClassInitialize()
        {
            GpioPin cePin = GpioController.GetDefault().OpenPin(22);

            DeviceInformationCollection devicesInfo = await DeviceInformation.FindAllAsync(SpiDevice.GetDeviceSelector("SPI0"));
            SpiDevice spiDevice = await SpiDevice.FromIdAsync(devicesInfo[0].Id, new SpiConnectionSettings(0));

            _radio = new Radio(cePin, spiDevice);
            _radio.Begin();
            _radio.OpenReadingPipe(1, _pipes[0]);
            _radio.StartListening();
            Debug.WriteLine(_radio.Configuration.GetDetails());
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
