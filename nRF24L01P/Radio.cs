using Common.Logging;
//using Newtonsoft.Json;
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
    public sealed class Radio : IRadio
    {
        private readonly object _syncRoot;
        private readonly GpioPin _powerPin;
        private readonly GpioPin _cePin;
        private readonly GpioPin _irqPin;
        private readonly ICommandProcessor _commandProcessor;
        private readonly ILoggerFactoryAdapter _loggerFactoryAdapter;
        private readonly ILog _logger;
        public event EventHandler<InterruptedEventArgs> Interrupted;
        public IRegisterContainer RegisterContainer { get; }
        public IConfiguration Configuration { get; }
        public ITransmitPipe TransmitPipe { get; }
        public IReceivePipeCollection ReceivePipes { get; }

        public Radio(ICommandProcessor commandProcessor, ILoggerFactoryAdapter loggerFactoryAdapter, GpioPin powerPin, GpioPin cePin, GpioPin irqPin = null)
        {

            _syncRoot = new object();
            _operatingMode = OperatingModes.PowerOff;
            _powerPin = powerPin;
            _powerPin.Write(GpioPinValue.Low);

            _cePin = cePin;
            _cePin.Write(GpioPinValue.Low);

            _loggerFactoryAdapter = loggerFactoryAdapter;
            _logger = _loggerFactoryAdapter.GetLogger(GetType());

            _commandProcessor = commandProcessor;
            _commandProcessor.LoggerFactory = _loggerFactoryAdapter;
            _commandProcessor.GetOperatingMode = () => OperatingMode;

            RegisterContainer = new RegisterContainer(_commandProcessor);
            OperatingMode = OperatingModes.PowerDown;
            RegisterContainer.ResetRegistersToDefault();

            Configuration = new Configuration(_commandProcessor, RegisterContainer);

            TransmitPipe = new TransmitPipe(Configuration, _commandProcessor, RegisterContainer, _logger, _cePin);
            ReceivePipes = new ReceivePipeCollection(Configuration, _commandProcessor, RegisterContainer);

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
            OperatingMode = OperatingModes.StandBy;

        }

        private void irqPin_ValueChanged(GpioPin sender, GpioPinValueChangedEventArgs args)
        {
            if (args.Edge != GpioPinEdge.FallingEdge) return;
            _logger.Trace("IRQ Event Received!");
            RegisterContainer.StatusRegister.Load();
            Interrupted?.Invoke(this, new InterruptedEventArgs { StatusRegister = RegisterContainer.StatusRegister });
            _irqPin.Write(GpioPinValue.High);
        }

        private OperatingModes _operatingMode;
        public OperatingModes OperatingMode
        {
            get
            {
                return _operatingMode;
            }
            set
            {
                lock (_syncRoot)
                {
                    if (value == _operatingMode)
                        return;
                    OperatingModes previousOperatingMode = _operatingMode;
                    _operatingMode = value;
                    _logger.InfoFormat("Switching OperatingMode from {0} to {1}.", previousOperatingMode, _operatingMode);
                    ConfigurationRegister configurationRegister = null;
                    if (previousOperatingMode != OperatingModes.PowerOff)
                    {
                        configurationRegister = RegisterContainer.ConfigurationRegister;
                        configurationRegister.Load();
                    }
                    switch (_operatingMode)
                    {
                        case OperatingModes.PowerOff:
                            if (previousOperatingMode == OperatingModes.PowerDown)
                            {
                                _powerPin.Write(GpioPinValue.Low);
                                break;
                            }
                            throw new InvalidOperatingModeTransitionException(previousOperatingMode, _operatingMode,
                                new[] { OperatingModes.PowerDown });
                        case OperatingModes.PowerDown:
                            if (previousOperatingMode == OperatingModes.PowerOff)
                            {
                                _powerPin.Write(GpioPinValue.High);
                                Delay.WaitMilliseconds(105);
                                RegisterContainer.ConfigurationRegister.ResetToDefault();
                                break;
                            }
                            if (previousOperatingMode == OperatingModes.StandBy)
                            {
                                configurationRegister.PowerUp = false;
                                configurationRegister.Save();
                                break;
                            }
                            throw new InvalidOperatingModeTransitionException(previousOperatingMode, _operatingMode,
                                new[] { OperatingModes.StandBy });
                        case OperatingModes.StandBy:
                            if (previousOperatingMode == OperatingModes.ReceiveMode ||
                                previousOperatingMode == OperatingModes.TransmitMode)
                            {
                                _cePin.Write(GpioPinValue.Low);
                                break;
                            }
                            if (previousOperatingMode == OperatingModes.PowerDown)
                            {
                                configurationRegister.PowerUp = true;
                                configurationRegister.Save();
                                Delay.WaitMilliseconds(2);
                                break;
                            }
                            throw new InvalidOperatingModeTransitionException(previousOperatingMode, _operatingMode,
                                new[]
                                {OperatingModes.PowerDown, OperatingModes.ReceiveMode, OperatingModes.TransmitMode});
                        case OperatingModes.TransmitMode:
                            if (previousOperatingMode == OperatingModes.StandBy)
                            {
                                bool checkOperatingMode = _commandProcessor.CheckOperatingMode;
                                _commandProcessor.CheckOperatingMode = false;

                                configurationRegister.PrimaryReceiveMode = false;
                                configurationRegister.Save();
                                _cePin.Write(GpioPinValue.High);
                                Delay.WaitMicroseconds(15);
                                //_cePin.Write(GpioPinValue.Low);
                                Delay.WaitMicroseconds(135);

                                _commandProcessor.CheckOperatingMode = checkOperatingMode;
                                break;
                            }
                            throw new InvalidOperatingModeTransitionException(previousOperatingMode, _operatingMode,
                                new[] { OperatingModes.StandBy });
                        case OperatingModes.ReceiveMode:
                            if (previousOperatingMode == OperatingModes.StandBy)
                            {
                                bool checkOperatingMode = _commandProcessor.CheckOperatingMode;
                                _commandProcessor.CheckOperatingMode = false;

                                configurationRegister.PrimaryReceiveMode = true;
                                configurationRegister.Save();
                                _cePin.Write(GpioPinValue.High);
                                Delay.WaitMicroseconds(135);

                                _commandProcessor.CheckOperatingMode = checkOperatingMode;
                                break;
                            }
                            throw new InvalidOperatingModeTransitionException(previousOperatingMode, _operatingMode,
                                new[] { OperatingModes.StandBy });
                    }
                }
            }
        }

        public string GetArduinoDetails()
        {
            return new ArduinoDetails(this,_loggerFactoryAdapter, Configuration, _commandProcessor, RegisterContainer).ToString();
        }

        public override string ToString()
        {
            var radio = new
            {
                OperatingMode = OperatingMode.GetName(),
                TransmitFIFO = TransmitPipe.FifoStatus.GetName(),
                ReceiveFIFO = ReceivePipes.FifoStatus.GetName(),
                ReceivePipes.ReceivedPowerDetector
            };

            string output = string.Format("Radio\r\n{0}\r\n{1}{2}",
                JsonHelper.ToJson(radio),
                Configuration,
                RegisterContainer);
            _logger.InfoFormat("Diagnostics Information\r\n{0}", output);
            return output;
        }

        public void Dispose()
        {
            _logger.Trace("Dispose");
            if (OperatingMode == OperatingModes.TransmitMode || OperatingMode == OperatingModes.ReceiveMode)
                OperatingMode = OperatingModes.StandBy;
            if (OperatingMode == OperatingModes.StandBy)
                OperatingMode = OperatingModes.PowerDown;
            if (OperatingMode == OperatingModes.PowerDown)
                OperatingMode = OperatingModes.PowerOff;

            _commandProcessor?.Dispose();
            _cePin?.Dispose();
            _irqPin?.Dispose();
        }

      
    }
}
