using Windows.Devices.Radios.nRF24L01P.Interfaces;

namespace Windows.Devices.Radios.nRF24L01P.Registers
{
    public class RegisterManager
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
        public RegisterCollection<RegisterBase> AllRegisters { get; }
        public RegisterCollection<AddressPipeRegister> ReceiveAddressPipeRegisters { get; }
        public RegisterCollection<ReceivePayloadWidthPipeRegister> ReceivePayloadWidthPipeRegisters { get; }
        public RegisterManager(ICommandProcessor commandProcessor)
        {
            ConfigurationRegister = new ConfigurationRegister(commandProcessor);
            EnableAutoAcknowledgementRegister = new EnableAutoAcknowledgementRegister(commandProcessor);
            EnableReceiveAddressRegister = new EnableReceiveAddressRegister(commandProcessor);
            AddressWidthRegister = new SetupAddressWidthRegister(commandProcessor);
            SetupRetransmissionRegister = new SetupRetransmissionRegister(commandProcessor);
            RfChannelRegister = new RfChannelRegister(commandProcessor);
            RfSetupRegister = new RfSetupRegister(commandProcessor);
            StatusRegister = new StatusRegister(commandProcessor);
            ObserveTransmitRegister = new ObserveTransmitRegister(commandProcessor);
            ReceivedPowerDetectorRegister = new ReceivedPowerDetectorRegister(commandProcessor);
            TransmitAddressRegister = new AddressPipeRegister(commandProcessor, RegisterAddresses.TX_ADDR, 0);
            FifoStatusRegister = new FifoStatusRegister(commandProcessor);
            DynamicPayloadLengthRegister = new DynamicPayloadLengthRegister(commandProcessor);
            FeatureRegister = new FeatureRegister(commandProcessor);
            ReceiveAddressPipeRegisters = new RegisterCollection<AddressPipeRegister>
            {
                {0, new AddressPipeRegister(commandProcessor, RegisterAddresses.RX_ADDR_P0, 0)},
                {1, new AddressPipeRegister(commandProcessor, RegisterAddresses.RX_ADDR_P1, 1)},
                {2, new AddressPipeRegister(commandProcessor, RegisterAddresses.RX_ADDR_P1, 2)},
                {3, new AddressPipeRegister(commandProcessor, RegisterAddresses.RX_ADDR_P1, 3)},
                {4, new AddressPipeRegister(commandProcessor, RegisterAddresses.RX_ADDR_P1, 4)},
                {5, new AddressPipeRegister(commandProcessor, RegisterAddresses.RX_ADDR_P1, 5)},
            };
            ReceivePayloadWidthPipeRegisters = new RegisterCollection<ReceivePayloadWidthPipeRegister>
            {
                {0, new ReceivePayloadWidthPipeRegister(commandProcessor, RegisterAddresses.RX_PW_P0, 0)},
                {1, new ReceivePayloadWidthPipeRegister(commandProcessor, RegisterAddresses.RX_PW_P1, 1)},
                {2, new ReceivePayloadWidthPipeRegister(commandProcessor, RegisterAddresses.RX_PW_P2, 2)},
                {3, new ReceivePayloadWidthPipeRegister(commandProcessor, RegisterAddresses.RX_PW_P3, 3)},
                {4, new ReceivePayloadWidthPipeRegister(commandProcessor, RegisterAddresses.RX_PW_P4, 4)},
                {5, new ReceivePayloadWidthPipeRegister(commandProcessor, RegisterAddresses.RX_PW_P5, 5)}
            };
            AllRegisters = new RegisterCollection<RegisterBase>
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
        }

        public void LoadRegisters() => AllRegisters.LoadAll();

        public void SaveRegisters() => AllRegisters.SaveAll();

        public override string ToString()
        {
            return AllRegisters.ToString();
        }
    }
}
