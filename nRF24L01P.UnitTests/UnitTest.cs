using System;
using System.Diagnostics;
using Windows.Devices.Enumeration;
using Windows.Devices.Gpio;
using Windows.Devices.Radios.nRF24L01P;
using Windows.Devices.Radios.nRF24L01P.Interfaces;
using Windows.Devices.Radios.nRF24L01P.Roles;
using Windows.Devices.Spi;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace nRF24L01P.UnitTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            GpioPin cePin = GpioController.GetDefault().OpenPin(22);

            DeviceInformationCollection devicesInfo = DeviceInformation.FindAllAsync(SpiDevice.GetDeviceSelector("SPI0")).GetAwaiter().GetResult();
            SpiDevice spiDevice = SpiDevice.FromIdAsync(devicesInfo[0].Id, new SpiConnectionSettings(0)).GetAwaiter().GetResult();

            IRadio radio = new Radio(cePin, spiDevice);
            radio.Initialize();
            Debug.WriteLine(radio.ToString());

            SendReceiveRole sendReceive = new SendReceiveRole();
            sendReceive.AttachDevice(radio);
            sendReceive.DataArrived += SendReceive_DataArrived;
            sendReceive.SendAddress = new byte[] { 0x54, 0x4d, 0x52, 0x68, 0x7C };
            sendReceive.ReceiveAddress = new byte[] { 0xAB, 0xCD, 0xAB, 0xCD, 0x71 };
            sendReceive.Start();
        }

        private void SendReceive_DataArrived(byte[] data)
        {
            
        }
    }
}
