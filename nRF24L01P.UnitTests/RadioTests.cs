using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.Gpio;
using Windows.Devices.Radios.nRF24L01P;
using Windows.Devices.Radios.nRF24L01P.Enums;
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
        private static ICommandProcessor _commandProcessor;

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            GpioController gpioController = GpioController.GetDefault();
            _irqPin = gpioController.InitGpioPin(25, GpioPinDriveMode.InputPullUp);
            _cePin = gpioController.InitGpioPin(22, GpioPinDriveMode.Output);

            DeviceInformationCollection devicesInfo = DeviceInformation.FindAllAsync(SpiDevice.GetDeviceSelector("SPI0")).GetAwaiter().GetResult();
            _spiDevice = SpiDevice.FromIdAsync(devicesInfo[0].Id, new SpiConnectionSettings(0)).GetAwaiter().GetResult();
            _commandProcessor = new CommandProcessor(_spiDevice); // new MockCommandProcessor();
            _radio = new Radio(_commandProcessor, _cePin, _irqPin);
        }

        [TestMethod]
        public void GetDiagnosticsTest()
        {
            Debug.WriteLine(_radio.GetDiagnostics());

            //SendReceiveRole sendReceive = new SendReceiveRole();
            //sendReceive.AttachDevice(_radio);
            //sendReceive.DataArrived += SendReceive_DataArrived;
            //sendReceive.SendAddress = new byte[] { 0x54, 0x4d, 0x52, 0x68, 0x7C };
            //sendReceive.ReceiveAddress = new byte[] { 0xAB, 0xCD, 0xAB, 0xCD, 0x71 };
            //sendReceive.Start();
        }

        [TestMethod]
        public void ConstantCarrierWaveOutputForTestinng()
        {
            Debug.WriteLine(_radio.GetDiagnostics());
            ConfigurationRegister configurationRegister = _radio.RegisterContainer.ConfigurationRegister;
            configurationRegister.PowerUp = true;
            configurationRegister.PrimaryReceiveMode = false;
            configurationRegister.Save();
            Task.Delay(150).Wait();

            RfSetupRegister rfSetupRegister = _radio.RegisterContainer.RfSetupRegister;
            rfSetupRegister.EnableContiuousCarrierTransmit = true;
            rfSetupRegister.ForcePllSignalLock = true;
            rfSetupRegister.PowerLevel = 3;
            rfSetupRegister.Save();

            RfChannelRegister rfChannelRegister = _radio.RegisterContainer.RfChannelRegister;
            rfChannelRegister.RfChannel = 1;
            rfChannelRegister.Save();

            _radio.Status = DeviceStatus.ReceiveMode;
        }

        private void SendReceive_DataArrived(object sender, byte[] data)
        {

        }
    }
}
