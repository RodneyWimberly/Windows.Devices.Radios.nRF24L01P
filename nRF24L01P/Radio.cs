using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Devices.Gpio;
using Windows.Devices.Radios.nRF24L01P.Enums;
using Windows.Devices.Radios.nRF24L01P.Interfaces;
using Windows.Devices.Radios.nRF24L01P.Registers;

namespace Windows.Devices.Radios.nRF24L01P
{
    /// <summary>
    /// Driver for nRF24L01(+) 2.4GHz Wireless Transceiver
    /// </summary>
    public class Radio : IRadio
    {
        private readonly GpioPin _cePin;
        private readonly GpioPin _irqPin;
        private readonly ICommandProcessor _commandProcessor;

        public IRegisterManager RegisterManager { get; }
        public IRadioConfiguration Configuration { get; }
        public ITransmitPipe TransmitPipe { get; }
        public IDictionary<int, IReceivePipe> ReceivePipes { get; }
        public event EventHandler<InterruptedEventArgs> Interrupted;

        public Radio(ICommandProcessor commandProcessor, GpioPin cePin, GpioPin irqPin = null)
        {
            _status = DeviceStatus.PowerDown;
            _cePin = cePin;
            EnableReceiver(false);

            // Set the ICommandProcessor GetDeviceStatus Function Pointer to GetStatus() 
            // (I wish I could just point it to _status field or Status property but can't
            // figure out how so I created the GetStatus() method to return _status.)
            // This is used for the ICommandProcessor to determine if its a proper status 
            // if before Executing Commands if ICommandProcessor.CheckStatus is true
            commandProcessor.GetDeviceStatus = GetStatus;
            _commandProcessor = commandProcessor;

            RegisterManager = new RegisterManager(_commandProcessor);
            RegisterManager.LoadRegisters();

            Configuration = new RadioConfiguration(_commandProcessor, RegisterManager);
            TransmitPipe = new TransmitPipe(Configuration, _commandProcessor, RegisterManager);
            ReceivePipes = new Dictionary<int, IReceivePipe>
            {
                {0, new ReceivePipe(Configuration, _commandProcessor, RegisterManager, 0)},
                {1, new ReceivePipe(Configuration, _commandProcessor, RegisterManager, 1)},
                {2, new ReceivePipe(Configuration, _commandProcessor, RegisterManager, 2)},
                {3, new ReceivePipe(Configuration, _commandProcessor, RegisterManager, 3)},
                {4, new ReceivePipe(Configuration, _commandProcessor, RegisterManager, 4)},
                {5, new ReceivePipe(Configuration, _commandProcessor, RegisterManager, 5)}
            };

            if (irqPin != null)
            {
                _irqPin = irqPin;
                _irqPin.DebounceTimeout = new TimeSpan(0, 0, 0, 0, 50);
                _irqPin.ValueChanged += irqPin_ValueChanged;
            }

            Task.Delay(1).Wait();
            Status = DeviceStatus.StandBy;
        }

        private void irqPin_ValueChanged(GpioPin sender, GpioPinValueChangedEventArgs args)
        {
            if (args.Edge != GpioPinEdge.FallingEdge) return;

            RegisterManager.StatusRegister.Load();
            Interrupted?.Invoke(this, new InterruptedEventArgs { StatusRegister = RegisterManager.StatusRegister });
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        public void Dispose()
        {
            _commandProcessor?.Dispose();
            _cePin?.Dispose();
            _irqPin?.Dispose();
        }

        public string GetDiagnostics()
        {
            return new Diagnostics(this, Configuration, _commandProcessor, RegisterManager).ToString();
        }

        private void EnableReceiver(bool enabled)
        {
            _cePin.Write(enabled ? GpioPinValue.High : GpioPinValue.Low);

            // Must allow the radio time to settle else configuration bits will not necessarily stick.
            // This is actually only required following power up but some settling time also appears to
            // be required after resets too. For full coverage, we'll always assume the worst.
            // Enabling 16b CRC is by far the most obvious case if the wrong timing is used - or skipped.
            // Technically we require 4.5ms + 14us as a worst case. We'll just call it 5ms for good measure.
            // WARNING: Delay is based on P-variant whereby non-P *may* require different timing.
            Task.Delay(5).Wait();
        }

        private DeviceStatus GetStatus() { return _status; }
        private DeviceStatus _status;
        public DeviceStatus Status
        {
            get
            {
                return _status;
            }
            set
            {
                if (value == _status)
                    return;
                DeviceStatus lastStatus = _status;
                _status = value;
                switch (_status)
                {
                    case DeviceStatus.Undefined:
                        throw new InvalidOperationException("Undefined Device Status");
                    case DeviceStatus.PowerDown:
                        if (lastStatus == DeviceStatus.StandBy)
                        {
                            RegisterManager.ConfigurationRegister.PowerUp = false;
                            RegisterManager.ConfigurationRegister.Save();
                            break;
                        }
                        throw new InvalidOperationException(
                            "Error status change, PowerDown should from StandBy mode only");
                    case DeviceStatus.StandBy:
                        if (lastStatus == DeviceStatus.ReceiveMode || lastStatus == DeviceStatus.TransmitMode)
                        {
                            EnableReceiver(false);
                            break;
                        }
                        if (lastStatus == DeviceStatus.PowerDown)
                        {
                            RegisterManager.ConfigurationRegister.PowerUp = true;
                            RegisterManager.ConfigurationRegister.Save();
                            Task.Delay(2).Wait();
                            break;
                        }
                        throw new InvalidOperationException(
                            "Error status change, StandBy should from PowerDown,TX or RX mode only");
                    case DeviceStatus.TransmitMode:
                        if (lastStatus == DeviceStatus.StandBy)
                        {
                            bool checkStatus = _commandProcessor.CheckStatus;
                            _commandProcessor.CheckStatus = false;
                            RegisterManager.ConfigurationRegister.PrimaryReceiveMode = false;
                            RegisterManager.ConfigurationRegister.Save();
                            _commandProcessor.CheckStatus = checkStatus;

                            EnableReceiver(true);
                            break;
                        }
                        throw new InvalidOperationException("Error status change, RXMode should from Standby mode only");
                    case DeviceStatus.ReceiveMode:
                        if (lastStatus == DeviceStatus.StandBy)
                        {
                            bool checkStatus = _commandProcessor.CheckStatus;
                            _commandProcessor.CheckStatus = false;
                            RegisterManager.ConfigurationRegister.PrimaryReceiveMode = true;
                            RegisterManager.ConfigurationRegister.Save();
                            _commandProcessor.CheckStatus = checkStatus;

                            EnableReceiver(true);
                            break;

                        }
                        throw new InvalidOperationException("Error status change, RXMode should from Standby mode only");
                }
            }
        }
    }
}
