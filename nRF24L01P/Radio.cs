using Newtonsoft.Json;
using System;
using Windows.Devices.Gpio;
using Windows.Devices.Radios.nRF24L01P.Enums;
using Windows.Devices.Radios.nRF24L01P.Exceptions;
using Windows.Devices.Radios.nRF24L01P.Extensions;
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
        public event EventHandler<InterruptedEventArgs> Interrupted;
        public IRegisterContainer RegisterContainer { get; }
        public IConfiguration Configuration { get; }
        public ITransmitPipe TransmitPipe { get; }
        public IReceivePipeCollection ReceivePipes { get; }

        public Radio(ICommandProcessor commandProcessor, GpioPin cePin, GpioPin irqPin = null)
        {
            _status = DeviceStatus.Undefined;

            _cePin = cePin;
            _cePin.Write(GpioPinValue.Low);

            _commandProcessor = commandProcessor;
            _commandProcessor.GetDeviceStatus = () => Status;

            RegisterContainer = new RegisterContainer(_commandProcessor);
            RegisterContainer.ResetRegistersToDefault();

            Configuration = new Configuration(_commandProcessor, RegisterContainer);

            TransmitPipe = new TransmitPipe(Configuration, _commandProcessor, RegisterContainer);
            ReceivePipes = new ReceivePipeCollection(Configuration, _commandProcessor, RegisterContainer);

            Status = DeviceStatus.PowerDown;

            bool useIrq = irqPin != null;
            if (useIrq)
            {
                _irqPin = irqPin;
                _irqPin.Write(GpioPinValue.High);
                _irqPin.ValueChanged += irqPin_ValueChanged;
            }
            ConfigurationRegister configurationRegister = RegisterContainer.ConfigurationRegister;
            configurationRegister.MaximunTransmitRetriesMask = !useIrq;
            configurationRegister.ReceiveDataReadyMask = !useIrq;
            configurationRegister.TransmitDataSentMask = !useIrq;
            configurationRegister.Save();
            Status = DeviceStatus.StandBy;
        }

        private void irqPin_ValueChanged(GpioPin sender, GpioPinValueChangedEventArgs args)
        {
            if (args.Edge != GpioPinEdge.FallingEdge) return;

            RegisterContainer.StatusRegister.Load();
            Interrupted?.Invoke(this, new InterruptedEventArgs { StatusRegister = RegisterContainer.StatusRegister });
            _irqPin.Write(GpioPinValue.High);
        }

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
                ConfigurationRegister configurationRegister = RegisterContainer.ConfigurationRegister;
                configurationRegister.Load();
                switch (_status)
                {
                    case DeviceStatus.Undefined:
                        throw new InvalidStatusTransitionException(lastStatus, _status, "you cannot transition into Undefined mode.");
                    case DeviceStatus.PowerDown:
                        if (lastStatus == DeviceStatus.Undefined)
                        {
                            Delay.WaitMilliseconds(105);
                            configurationRegister.ResetToDefault();
                            break;
                        }
                        if (lastStatus == DeviceStatus.StandBy)
                        {
                            configurationRegister.PowerUp = false;
                            configurationRegister.Save();
                            break;
                        }
                        throw new InvalidStatusTransitionException(lastStatus, _status, "you can only transition into PowerDown mode from StandBy mode.");
                    case DeviceStatus.StandBy:
                        if (lastStatus == DeviceStatus.ReceiveMode || lastStatus == DeviceStatus.TransmitMode)
                        {
                            _cePin.Write(GpioPinValue.Low);
                            break;
                        }
                        if (lastStatus == DeviceStatus.PowerDown)
                        {
                            configurationRegister.PowerUp = true;
                            configurationRegister.Save();
                            Delay.WaitMilliseconds(2);
                            break;
                        }
                        throw new InvalidStatusTransitionException(lastStatus, _status, "you can only transition into StandBy mode from PowerDown, TransmitMode, or ReceiveMode mode.");
                    case DeviceStatus.TransmitMode:
                        if (lastStatus == DeviceStatus.StandBy)
                        {
                            bool checkStatus = _commandProcessor.CheckStatus;
                            _commandProcessor.CheckStatus = false;

                            configurationRegister.PrimaryReceiveMode = false;
                            configurationRegister.Save();
                            _cePin.Write(GpioPinValue.High);
                            Delay.WaitMicroseconds(10);
                            //_cePin.Write(GpioPinValue.Low);
                            Delay.WaitMicroseconds(130);

                            _commandProcessor.CheckStatus = checkStatus;
                            break;
                        }
                        throw new InvalidStatusTransitionException(lastStatus, _status, "you can only transition into TransmitMode mode from Standby mode.");
                    case DeviceStatus.ReceiveMode:
                        if (lastStatus == DeviceStatus.StandBy)
                        {
                            bool checkStatus = _commandProcessor.CheckStatus;
                            _commandProcessor.CheckStatus = false;

                            configurationRegister.PrimaryReceiveMode = true;
                            configurationRegister.Save();
                            _cePin.Write(GpioPinValue.High);
                            Delay.WaitMicroseconds(130);

                            _commandProcessor.CheckStatus = checkStatus;
                            break;
                        }
                        throw new InvalidStatusTransitionException(lastStatus, _status, "you can only transition into ReceiveMode mode from Standby mode.");
                }
            }
        }

        public string GetArduinoDetails()
        {
            return new ArduinoDetails(this, Configuration, _commandProcessor, RegisterContainer).ToString();
        }

        public override string ToString()
        {
            var radio = new
            {
                Status = Status.GetName(),
                TransmitFIFO = TransmitPipe.FifoStatus.GetName(),
                ReceiveFIFO = ReceivePipes.FifoStatus.GetName(),
                ReceivePipes.ReceivedPowerDetector
            };
            return string.Format("Radio\r\n{0}\r\n{1}{2}",
                JsonConvert.SerializeObject(radio, Formatting.None),
                Configuration,
                RegisterContainer);
        }

        public void Dispose()
        {
            _commandProcessor?.Dispose();
            _cePin?.Dispose();
            _irqPin?.Dispose();
        }
    }
}
