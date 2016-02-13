using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Windows.Devices.Gpio;
using Windows.Devices.Radios.nRF24L01P.Enums;
using Windows.Devices.Radios.nRF24L01P.Interfaces;
using Windows.Devices.Radios.nRF24L01P.Registers;
using Windows.Devices.Spi;

namespace Windows.Devices.Radios.nRF24L01P
{
    /// <summary>
    /// Driver for nRF24L01(+) 2.4GHz Wireless Transceiver
    /// </summary>
    public class Radio : IRadio
    {
        private readonly GpioPin _cePin;

        public ICommandProcessor CommandProcessor { get; }

        public IRadioConfiguration Configuration { get;  }

        public ITransmitPipe TransmitPipe { get; }

        public IDictionary<int, IReceivePipe> ReceivePipes { get;  }

        public Radio(GpioPin cePin, SpiDevice spiDevice)
        {
            _cePin = cePin;
            _cePin.SetDriveMode(GpioPinDriveMode.Output);
            ChipEnable(false);

            CommandProcessor = new CommandProcessor(spiDevice, this, false);
            Configuration = new RadioConfiguration(CommandProcessor);
            CommandProcessor.LoadStatusRegister = Configuration.Registers.StatusRegister.Load;
            TransmitPipe = new TransmitPipe(Configuration, CommandProcessor);
            ReceivePipes = new Dictionary<int, IReceivePipe>
            {
                {0, new ReceivePipe(Configuration, CommandProcessor, 0)},
                {1, new ReceivePipe(Configuration, CommandProcessor, 1)},
                {2, new ReceivePipe(Configuration, CommandProcessor, 2)},
                {3, new ReceivePipe(Configuration, CommandProcessor, 3)},
                {4, new ReceivePipe(Configuration, CommandProcessor, 4)},
                {5, new ReceivePipe(Configuration, CommandProcessor, 5)}
            };
            _status = DeviceStatus.PowerDown;
        }

        public override string ToString()
        {
            return string.Format("{0}\r\n\r\n{1}", 
                new Diagnostics(Configuration, CommandProcessor).GetDetails(),
                 JsonConvert.SerializeObject(this, Formatting.Indented));
        }

        public void Initialize()
        {
            // Shut down device for config
            Status = DeviceStatus.StandBy;
            ChipEnable(false);

            // Set 1500uS (minimum for 32B payload in ESB@250KBPS) timeouts, to make testing a little easier
            // WARNING: If this is ever lowered, either 250KBS mode with AA is broken or maximum packet
            // sizes must never be used. See documentation for a more complete explanation.
            Configuration.AutoRetransmitDelay = AutoRetransmitDelays.Delay1500uS;
            Configuration.AutoRetransmitCount = 15;

            // Restore our default PA level
            Configuration.PowerLevel = PowerLevels.Max;

            // Attempt to set DataRate to 250Kbps to determine if this is a plus model
            Configuration.DataRate = DataRates.DataRate250Kbps;
            Configuration.IsPlusModel = Configuration.DataRate == DataRates.DataRate250Kbps;

            // Then set the data rate to the slowest (and most reliable) speed supported by all hardware.
            Configuration.DataRate = DataRates.DataRate1Mbps;

            // Initialize CRC and request 2-byte (16bit) CRC
            Configuration.CrcEncodingScheme = CrcEncodingSchemes.DualBytes;
            Configuration.CrcEnabled = true;

            // Disable dynamic payload lengths
            Configuration.DynamicPayloadLengthEnabled = false;

            // Reset current status
            // Notice reset and flush is the last thing we do
            StatusRegister statusRegister = Configuration.Registers.StatusRegister;
            statusRegister.ReceiveDataReady = false;
            statusRegister.TransmitDataSent = false;
            statusRegister.MaximunTransmitRetries = false;
            statusRegister.Save();

            // Set up default configuration.  Callers can always change it later.
            // This channel should be universally safe and not bleed over into adjacent spectrum.
            Configuration.Channel = 76;

            ReceivePipes[0].FlushBuffer();
            TransmitPipe.FlushBuffer();
        }

        public void ChipEnable(bool enabled)
        {
            _cePin.Write(enabled ? GpioPinValue.High : GpioPinValue.Low);

            // Must allow the radio time to settle else configuration bits will not necessarily stick.
            // This is actually only required following power up but some settling time also appears to
            // be required after resets too. For full coverage, we'll always assume the worst.
            // Enabling 16b CRC is by far the most obvious case if the wrong timing is used - or skipped.
            // Technically we require 4.5ms + 14us as a worst case. We'll just call it 5ms for good measure.
            // WARNING: Delay is based on P-variant whereby non-P *may* require different timing.
            Utilities.DelayMicroseconds(5000);
        }

        public bool ChannelReceivedPowerDetector
        {
            get
            {
                Configuration.Registers.ReceivedPowerDetectorRegister.Load();
                return Configuration.Registers.ReceivedPowerDetectorRegister.ReceivedPowerDetector;
            }
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
                switch (_status)
                {
                    case DeviceStatus.Undefined:
                        throw new InvalidOperationException("WTF???");
                    case DeviceStatus.PowerDown:
                        if (lastStatus == DeviceStatus.StandBy)
                        {
                            Configuration.Registers.ConfigurationRegister.PowerUp = false;
                            Configuration.Registers.ConfigurationRegister.Save();
                            break;
                        }
                        throw new InvalidOperationException(
                            "Error status change, PowerDown should from StandBy mode only");
                    case DeviceStatus.StandBy:
                        if (lastStatus == DeviceStatus.ReceiveMode || lastStatus == DeviceStatus.TransmitMode)
                        {
                            ChipEnable(false);
                            break;
                        }
                        if (lastStatus == DeviceStatus.PowerDown)
                        {
                            Configuration.Registers.ConfigurationRegister.PowerUp = true;
                            Configuration.Registers.ConfigurationRegister.Save();
                            Utilities.DelayMicroseconds(2000);
                            break;
                        }
                        throw new InvalidOperationException(
                            "Error status change, StandBy should from PowerDown,TX or RX mode only");
                    case DeviceStatus.TransmitMode:
                        if (lastStatus == DeviceStatus.StandBy)
                        {
                            CommandProcessor.CheckStatus = false;
                            Configuration.Registers.ConfigurationRegister.PrimaryReceiveMode = false;
                            Configuration.Registers.ConfigurationRegister.Save();
                            CommandProcessor.CheckStatus = true;

                            ChipEnable(true);
                            Utilities.DelayMicroseconds(1000); 
                            break;
                        }
                        throw new InvalidOperationException("Error status change, RXMode should from Standby mode only");
                    case DeviceStatus.ReceiveMode:
                        if (lastStatus == DeviceStatus.StandBy)
                        {
                            CommandProcessor.CheckStatus = false;
                            Configuration.Registers.ConfigurationRegister.PrimaryReceiveMode = true;
                            Configuration.Registers.ConfigurationRegister.Save();
                            CommandProcessor.CheckStatus = false;
                            ChipEnable(true);
                            Utilities.DelayMicroseconds(1000); 
                            break;

                        }
                        throw new InvalidOperationException("Error status change, RXMode should from Standby mode only");
                    default:
                        break;
                }
            }
        }
    }
}
