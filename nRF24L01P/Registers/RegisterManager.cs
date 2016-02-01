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
        public RegisterManager(Radio radio)
        {
            ConfigurationRegister = new ConfigurationRegister(radio);
            EnableAutoAcknowledgementRegister = new EnableAutoAcknowledgementRegister(radio);
            EnableReceiveAddressRegister = new EnableReceiveAddressRegister(radio);
            AddressWidthRegister = new SetupAddressWidthRegister(radio);
            SetupRetransmissionRegister = new SetupRetransmissionRegister(radio);
            RfChannelRegister = new RfChannelRegister(radio);
            RfSetupRegister = new RfSetupRegister(radio);
            StatusRegister = new StatusRegister(radio);
            ObserveTransmitRegister = new ObserveTransmitRegister(radio);
            ReceivedPowerDetectorRegister = new ReceivedPowerDetectorRegister(radio);
            TransmitAddressRegister = new AddressPipeRegister(radio, Addresses.TX_ADDR, 0);
            FifoStatusRegister = new FifoStatusRegister(radio);
            DynamicPayloadLengthRegister = new DynamicPayloadLengthRegister(radio);
            FeatureRegister = new FeatureRegister(radio);
            ReceiveAddressPipeRegisters = new RegisterCollection<AddressPipeRegister>
            {
                {0, new AddressPipeRegister(radio, Addresses.RX_ADDR_P0, 0)},
                {1, new AddressPipeRegister(radio, Addresses.RX_ADDR_P1, 1)},
                {2, new AddressPipeRegister(radio, Addresses.RX_ADDR_P1, 2)},
                {3, new AddressPipeRegister(radio, Addresses.RX_ADDR_P1, 3)},
                {4, new AddressPipeRegister(radio, Addresses.RX_ADDR_P1, 4)},
                {5, new AddressPipeRegister(radio, Addresses.RX_ADDR_P1, 5)},
            };
            ReceivePayloadWidthPipeRegisters = new RegisterCollection<ReceivePayloadWidthPipeRegister>
            {
                {0, new ReceivePayloadWidthPipeRegister(radio, Addresses.RX_PW_P0, 0)},
                {1, new ReceivePayloadWidthPipeRegister(radio, Addresses.RX_PW_P1, 1)},
                {2, new ReceivePayloadWidthPipeRegister(radio, Addresses.RX_PW_P2, 2)},
                {3, new ReceivePayloadWidthPipeRegister(radio, Addresses.RX_PW_P3, 3)},
                {4, new ReceivePayloadWidthPipeRegister(radio, Addresses.RX_PW_P4, 4)},
                {5, new ReceivePayloadWidthPipeRegister(radio, Addresses.RX_PW_P5, 5)}
            };
            AllRegisters = new RegisterCollection<RegisterBase>
            {
                {Addresses.CONFIG, ConfigurationRegister},
                {Addresses.EN_AA, EnableAutoAcknowledgementRegister},
                {Addresses.EN_RXADDR, EnableReceiveAddressRegister},
                {Addresses.SETUP_AW, AddressWidthRegister},
                {Addresses.SETUP_RETR, SetupRetransmissionRegister},
                {Addresses.RF_CH, RfChannelRegister},
                {Addresses.RF_SETUP, RfSetupRegister},
                {Addresses.STATUS, StatusRegister},
                {Addresses.OBSERVE_TX, ObserveTransmitRegister},
                {Addresses.RPD, ReceivedPowerDetectorRegister},
                {Addresses.RX_ADDR_P0, ReceiveAddressPipeRegisters[0]},
                {Addresses.RX_ADDR_P1, ReceiveAddressPipeRegisters[1]},
                {Addresses.RX_ADDR_P2, ReceiveAddressPipeRegisters[2]},
                {Addresses.RX_ADDR_P3, ReceiveAddressPipeRegisters[3]},
                {Addresses.RX_ADDR_P4, ReceiveAddressPipeRegisters[4]},
                {Addresses.RX_ADDR_P5, ReceiveAddressPipeRegisters[5]},
                {Addresses.TX_ADDR, TransmitAddressRegister},
                {Addresses.RX_PW_P0, ReceivePayloadWidthPipeRegisters[0]},
                {Addresses.RX_PW_P1, ReceivePayloadWidthPipeRegisters[1]},
                {Addresses.RX_PW_P2, ReceivePayloadWidthPipeRegisters[2]},
                {Addresses.RX_PW_P3, ReceivePayloadWidthPipeRegisters[3]},
                {Addresses.RX_PW_P4, ReceivePayloadWidthPipeRegisters[4]},
                {Addresses.RX_PW_P5, ReceivePayloadWidthPipeRegisters[5]},
                {Addresses.FIFO_STATUS, FifoStatusRegister},
                {Addresses.DYNPD, DynamicPayloadLengthRegister},
                {Addresses.FEATURE, FeatureRegister}
            };
        }

        public void LoadRegisters() => AllRegisters.LoadAll();

        public void SaveRegisters() => AllRegisters.SaveAll();
    }
}
