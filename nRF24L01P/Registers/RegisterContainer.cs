using Common.Logging;
using Windows.Devices.Radios.nRF24L01P.Interfaces;

namespace Windows.Devices.Radios.nRF24L01P.Registers
{
    public class RegisterContainer : IRegisterContainer
    {
        public ConfigurationRegister ConfigurationRegister { get; }
        public EnableAutoAcknowledgementRegister EnableAutoAcknowledgementRegister { get; }
        public EnableReceiveAddressRegister EnableReceiveAddressRegister { get; }
        public SetupAddressWidthRegister AddressWidthRegister { get; }
        public SetupRetransmissionRegister SetupRetransmissionRegister { get; }
        public RfChannelRegister RfChannelRegister { get; }
        public RfSetupRegister RfSetupRegister { get; }
        public StatusRegister StatusRegister { get; }
        public ObserveTransmitRegister ObserveTransmitRegister { get; }
        public ReceivedPowerDetectorRegister ReceivedPowerDetectorRegister { get; }
        public AddressPipeRegister TransmitAddressRegister { get; }
        public FifoStatusRegister FifoStatusRegister { get; }
        public DynamicPayloadLengthRegister DynamicPayloadLengthRegister { get; }
        public FeatureRegister FeatureRegister { get; }
        public IRegisterCollection<IRegister> AllRegisters { get; }
        public IRegisterCollection<AddressPipeRegister> ReceiveAddressPipeRegisters { get; }
        public IRegisterCollection<ReceivePayloadWidthPipeRegister> ReceivePayloadWidthPipeRegisters { get; }
        public RegisterContainer(ILoggerFactoryAdapter loggerFactoryAdapter, ICommandProcessor commandProcessor)
        {
            ConfigurationRegister = new ConfigurationRegister(loggerFactoryAdapter, commandProcessor);
            EnableAutoAcknowledgementRegister = new EnableAutoAcknowledgementRegister(loggerFactoryAdapter, commandProcessor);
            EnableReceiveAddressRegister = new EnableReceiveAddressRegister(loggerFactoryAdapter, commandProcessor);
            AddressWidthRegister = new SetupAddressWidthRegister(loggerFactoryAdapter, commandProcessor);
            SetupRetransmissionRegister = new SetupRetransmissionRegister(loggerFactoryAdapter, commandProcessor);
            RfChannelRegister = new RfChannelRegister(loggerFactoryAdapter, commandProcessor);
            RfSetupRegister = new RfSetupRegister(loggerFactoryAdapter, commandProcessor);
            StatusRegister = new StatusRegister(loggerFactoryAdapter, commandProcessor);
            ObserveTransmitRegister = new ObserveTransmitRegister(loggerFactoryAdapter, commandProcessor);
            ReceivedPowerDetectorRegister = new ReceivedPowerDetectorRegister(loggerFactoryAdapter, commandProcessor);
            TransmitAddressRegister = new AddressPipeRegister(loggerFactoryAdapter, commandProcessor, RegisterAddresses.TX_ADDR, RegisterDefaults.TX_ADDR, 0, "TX_ADDR");
            FifoStatusRegister = new FifoStatusRegister(loggerFactoryAdapter, commandProcessor);
            DynamicPayloadLengthRegister = new DynamicPayloadLengthRegister(loggerFactoryAdapter, commandProcessor);
            FeatureRegister = new FeatureRegister(loggerFactoryAdapter, commandProcessor);
            ReceiveAddressPipeRegisters = new RegisterCollection<AddressPipeRegister>
            {
                {0, new AddressPipeRegister(loggerFactoryAdapter, commandProcessor, RegisterAddresses.RX_ADDR_P0, RegisterDefaults.RX_ADDR_P0, 0, "RX_ADDR_P0")},
                {1, new AddressPipeRegister(loggerFactoryAdapter, commandProcessor, RegisterAddresses.RX_ADDR_P1, RegisterDefaults.RX_ADDR_P1, 1, "RX_ADDR_P1")},
                {2, new AddressPipeRegister(loggerFactoryAdapter, commandProcessor, RegisterAddresses.RX_ADDR_P2, RegisterDefaults.RX_ADDR_P2, 2, "RX_ADDR_P2")},
                {3, new AddressPipeRegister(loggerFactoryAdapter, commandProcessor, RegisterAddresses.RX_ADDR_P3, RegisterDefaults.RX_ADDR_P3, 3, "RX_ADDR_P3")},
                {4, new AddressPipeRegister(loggerFactoryAdapter, commandProcessor, RegisterAddresses.RX_ADDR_P4, RegisterDefaults.RX_ADDR_P4, 4, "RX_ADDR_P4")},
                {5, new AddressPipeRegister(loggerFactoryAdapter, commandProcessor, RegisterAddresses.RX_ADDR_P5, RegisterDefaults.RX_ADDR_P5, 5, "RX_ADDR_P5")},
            };
            ReceivePayloadWidthPipeRegisters = new RegisterCollection<ReceivePayloadWidthPipeRegister>
            {
                {0, new ReceivePayloadWidthPipeRegister(loggerFactoryAdapter, commandProcessor, RegisterAddresses.RX_PW_P0, RegisterDefaults.RX_PW_P0, 0, "RX_PW_P0")},
                {1, new ReceivePayloadWidthPipeRegister(loggerFactoryAdapter, commandProcessor, RegisterAddresses.RX_PW_P1, RegisterDefaults.RX_PW_P1, 1, "RX_PW_P1")},
                {2, new ReceivePayloadWidthPipeRegister(loggerFactoryAdapter, commandProcessor, RegisterAddresses.RX_PW_P2, RegisterDefaults.RX_PW_P2, 2, "RX_PW_P2")},
                {3, new ReceivePayloadWidthPipeRegister(loggerFactoryAdapter, commandProcessor, RegisterAddresses.RX_PW_P3, RegisterDefaults.RX_PW_P3, 3, "RX_PW_P3")},
                {4, new ReceivePayloadWidthPipeRegister(loggerFactoryAdapter, commandProcessor, RegisterAddresses.RX_PW_P4, RegisterDefaults.RX_PW_P4, 4, "RX_PW_P4")},
                {5, new ReceivePayloadWidthPipeRegister(loggerFactoryAdapter, commandProcessor, RegisterAddresses.RX_PW_P5, RegisterDefaults.RX_PW_P5, 5, "RX_PW_P5")}
            };
            AllRegisters = new RegisterCollection<IRegister>
            {
                {RegisterAddresses.CONFIG, ConfigurationRegister},
                {RegisterAddresses.EN_AA, EnableAutoAcknowledgementRegister},
                {RegisterAddresses.EN_RXADDR, EnableReceiveAddressRegister},
                {RegisterAddresses.SETUP_AW, AddressWidthRegister},
                {RegisterAddresses.SETUP_RETR, SetupRetransmissionRegister},
                {RegisterAddresses.RF_CH, RfChannelRegister},
                {RegisterAddresses.RF_SETUP, RfSetupRegister},
                {RegisterAddresses.STATUS, StatusRegister},
                {RegisterAddresses.OBSERVE_TX, ObserveTransmitRegister},
                {RegisterAddresses.RPD, ReceivedPowerDetectorRegister},
                {RegisterAddresses.RX_ADDR_P0, ReceiveAddressPipeRegisters[0]},
                {RegisterAddresses.RX_ADDR_P1, ReceiveAddressPipeRegisters[1]},
                {RegisterAddresses.RX_ADDR_P2, ReceiveAddressPipeRegisters[2]},
                {RegisterAddresses.RX_ADDR_P3, ReceiveAddressPipeRegisters[3]},
                {RegisterAddresses.RX_ADDR_P4, ReceiveAddressPipeRegisters[4]},
                {RegisterAddresses.RX_ADDR_P5, ReceiveAddressPipeRegisters[5]},
                {RegisterAddresses.TX_ADDR, TransmitAddressRegister},
                {RegisterAddresses.RX_PW_P0, ReceivePayloadWidthPipeRegisters[0]},
                {RegisterAddresses.RX_PW_P1, ReceivePayloadWidthPipeRegisters[1]},
                {RegisterAddresses.RX_PW_P2, ReceivePayloadWidthPipeRegisters[2]},
                {RegisterAddresses.RX_PW_P3, ReceivePayloadWidthPipeRegisters[3]},
                {RegisterAddresses.RX_PW_P4, ReceivePayloadWidthPipeRegisters[4]},
                {RegisterAddresses.RX_PW_P5, ReceivePayloadWidthPipeRegisters[5]},
                {RegisterAddresses.FIFO_STATUS, FifoStatusRegister},
                {RegisterAddresses.DYNPD, DynamicPayloadLengthRegister},
                {RegisterAddresses.FEATURE, FeatureRegister}
            };

            // Set the LoadStatusRegister Action of the ICommandProcessor to call 
            // StatusRegister.Load. This will ensure that the Status register is 
            // automatically updated after call ICommandProcessor.ExecuteCommand()
            commandProcessor.LoadStatusRegister = StatusRegister.Load;
        }

        public void LoadRegisters() => AllRegisters.LoadAll();

        public void ResetRegistersToDefault() => AllRegisters.ResetAllToDefault();

        public void SaveRegisters(bool force = false) => AllRegisters.SaveAll(force);

        public override string ToString() => AllRegisters.ToString();
    }
}
