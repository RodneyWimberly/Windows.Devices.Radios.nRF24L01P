using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.Gpio;
using Windows.Devices.Radios.nRF24L01P;
using Windows.Devices.Radios.nRF24L01P.Interfaces;
using Windows.Devices.Radios.nRF24L01P.Registers;
using Windows.Devices.Radios.nRF24L01P.Roles;
using Windows.Devices.Spi;

namespace nRF24L01P.UnitTests
{
    [TestClass]
    public class UnitTest1
    {
        private static GpioPin _irqPin;
        private static GpioPin _cePin;
        private static SpiDevice _spiDevice;
        private static IRadio _radio;

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            GpioController gpioController = GpioController.GetDefault();
            _irqPin = gpioController.InitGpioPin(25, GpioPinDriveMode.InputPullUp);
            _cePin = gpioController.InitGpioPin(22, GpioPinDriveMode.Output);

            DeviceInformationCollection devicesInfo = DeviceInformation.FindAllAsync(SpiDevice.GetDeviceSelector("SPI0")).GetAwaiter().GetResult();
            _spiDevice = SpiDevice.FromIdAsync(devicesInfo[0].Id, new SpiConnectionSettings(0)).GetAwaiter().GetResult();

            _radio = new Radio(_spiDevice, _cePin, _irqPin);
        }

        [TestMethod]
        public void TestMethod1()
        {
            Debug.WriteLine(_radio.GetDiagnostics());

            SendReceiveRole sendReceive = new SendReceiveRole();
            sendReceive.AttachDevice(_radio);
            sendReceive.DataArrived += SendReceive_DataArrived;
            sendReceive.SendAddress = new byte[] { 0x54, 0x4d, 0x52, 0x68, 0x7C };
            sendReceive.ReceiveAddress = new byte[] { 0xAB, 0xCD, 0xAB, 0xCD, 0x71 };
            sendReceive.Start();
        }

        [TestMethod]
        public void ConstantCarrierWaveOutputForTestinng()
        {
            Debug.WriteLine(_radio.GetDiagnostics());
            ConfigurationRegister configurationRegister = _radio.Configuration.Registers.ConfigurationRegister;
            configurationRegister.PowerUp = true;
            configurationRegister.PrimaryReceiveMode = false;
            configurationRegister.Save();
            Task.Delay(150).Wait();

            RfSetupRegister rfSetupRegister = _radio.Configuration.Registers.RfSetupRegister;
            rfSetupRegister.EnableContiuousCarrierTransmit = true;
            rfSetupRegister.ForcePllSignalLock = true;
            rfSetupRegister.PowerLevelHigh = true;
            rfSetupRegister.PowerLevelLow = true;
            rfSetupRegister.Save();

            RfChannelRegister rfChannelRegister = _radio.Configuration.Registers.RfChannelRegister;
            rfChannelRegister.RfChannel = 76;
            rfChannelRegister.Save();

            _radio.ChipEnable(true);
        }

        private void SendReceive_DataArrived(object sender, byte[] data)
        {

        }
    }
}
