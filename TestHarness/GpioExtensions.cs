using System.IO;
using Windows.Devices.Gpio;

namespace nRF24L01P.TestHarness
{
    public static class GpioExtensions
    {
        public static GpioPin InitGpioPin(this GpioController controller,
            int pinNumber,
            GpioPinDriveMode driverMode,
            GpioSharingMode sharingMode)
        {
            GpioPin pin;
            GpioOpenStatus openStatus;
            if (!controller.TryOpenPin(pinNumber, sharingMode, out pin, out openStatus))
                throw new IOException($"Unable to open Pin Number {pinNumber}!");
            pin.Write(GpioPinValue.Low);
            pin.SetDriveMode(driverMode);
            if (openStatus != GpioOpenStatus.PinOpened)
                throw new IOException($"Pin Number {pinNumber} was opened but has an open status of [{openStatus}]!");
            return pin;
        }
    }
}