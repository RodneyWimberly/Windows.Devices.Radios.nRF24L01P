using System;
using System.Collections.Generic;
using System.Text;

namespace Windows.Devices.Radios.nRF24L01.Registers
{
    public class RegisterCollection : Dictionary<byte, RegisterBase>
    {
        private Radio _radio;

        public ConfigurationRegister ConfigurationRegister { get; private set; }
        public EnableAutoAcknowledgementRegister EnableAutoAcknowledgementRegister { get; private set; }
        public EnableReceiveAddressRegister EnableReceiveAddressRegister { get; private set; }
        public SetupAddressWidthRegister AddressWidthRegister { get; private set; }
        public SetupRetransmissionRegister SetupRetransmissionRegister { get; private set; }
        public RfChannelRegister RfChannelRegister { get; private set; }
        public RfSetupRegister RfSetupRegister { get; private set; }
        public StatusRegister StatusRegister { get; private set; }
        public ObserveTransmitRegister ObserveTransmitRegister { get; private set; }
        public ReceivedPowerDetectorRegister ReceivedPowerDetectorRegister { get; private set; }
        public ReceiveAddressPipe0Register ReceiveAddressPipe0Register { get; private set; }
        public ReceiveAddressPipe1Register ReceiveAddressPipe1Register { get; private set; }
        public ReceiveAddressPipe2Register ReceiveAddressPipe2Register { get; private set; }
        public ReceiveAddressPipe3Register ReceiveAddressPipe3Register { get; private set; }
        public ReceiveAddressPipe4Register ReceiveAddressPipe4Register { get; private set; }
        public ReceiveAddressPipe5Register ReceiveAddressPipe5Register { get; private set; }
        public TransmitAddressRegister TransmitAddressRegister { get; private set; }
        public ReceivePayloadWidthPipe0Register ReceivePayloadWidthPipe0Register { get; private set; }
        public ReceivePayloadWidthPipe1Register ReceivePayloadWidthPipe1Register { get; private set; }
        public ReceivePayloadWidthPipe2Register ReceivePayloadWidthPipe2Register { get; private set; }
        public ReceivePayloadWidthPipe3Register ReceivePayloadWidthPipe3Register { get; private set; }
        public ReceivePayloadWidthPipe4Register ReceivePayloadWidthPipe4Register { get; private set; }
        public ReceivePayloadWidthPipe5Register ReceivePayloadWidthPipe5Register { get; private set; }
        public FifoStatusRegister FifoStatusRegister { get; private set; }
        public DynamicPayloadLengthRegister DynamicPayloadLengthRegister { get; private set; }
        public FeatureRegister FeatureRegister { get; private set; }

        public RegisterCollection(Radio radio)
        {
            _radio = radio;
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
            ReceiveAddressPipe0Register = new ReceiveAddressPipe0Register(radio);
            ReceiveAddressPipe1Register = new ReceiveAddressPipe1Register(radio);
            ReceiveAddressPipe2Register = new ReceiveAddressPipe2Register(radio);
            ReceiveAddressPipe3Register = new ReceiveAddressPipe3Register(radio);
            ReceiveAddressPipe4Register = new ReceiveAddressPipe4Register(radio);
            ReceiveAddressPipe5Register = new ReceiveAddressPipe5Register(radio);
            TransmitAddressRegister = new TransmitAddressRegister(radio);
            ReceivePayloadWidthPipe0Register = new ReceivePayloadWidthPipe0Register(radio);
            ReceivePayloadWidthPipe1Register = new ReceivePayloadWidthPipe1Register(radio);
            ReceivePayloadWidthPipe2Register = new ReceivePayloadWidthPipe2Register(radio);
            ReceivePayloadWidthPipe3Register = new ReceivePayloadWidthPipe3Register(radio);
            ReceivePayloadWidthPipe4Register = new ReceivePayloadWidthPipe4Register(radio);
            ReceivePayloadWidthPipe5Register = new ReceivePayloadWidthPipe5Register(radio);
            FifoStatusRegister = new FifoStatusRegister(radio);
            DynamicPayloadLengthRegister = new DynamicPayloadLengthRegister(radio);
            FeatureRegister = new FeatureRegister(radio);

            Add(Addresses.CONFIG, ConfigurationRegister);
            Add(Addresses.EN_AA, EnableAutoAcknowledgementRegister);
            Add(Addresses.EN_RXADDR, EnableReceiveAddressRegister);
            Add(Addresses.SETUP_AW, AddressWidthRegister);
            Add(Addresses.SETUP_RETR, SetupRetransmissionRegister);
            Add(Addresses.RF_CH, RfChannelRegister);
            Add(Addresses.RF_SETUP, RfSetupRegister);
            Add(Addresses.STATUS, StatusRegister);
            Add(Addresses.OBSERVE_TX, ObserveTransmitRegister);
            Add(Addresses.RPD, ReceivedPowerDetectorRegister);
            Add(Addresses.RX_ADDR_P0, ReceiveAddressPipe0Register);
            Add(Addresses.RX_ADDR_P1, ReceiveAddressPipe1Register);
            Add(Addresses.RX_ADDR_P2, ReceiveAddressPipe2Register);
            Add(Addresses.RX_ADDR_P3, ReceiveAddressPipe3Register);
            Add(Addresses.RX_ADDR_P4, ReceiveAddressPipe4Register);
            Add(Addresses.RX_ADDR_P5, ReceiveAddressPipe5Register);
            Add(Addresses.TX_ADDR, TransmitAddressRegister);
            Add(Addresses.RX_PW_P0, ReceivePayloadWidthPipe0Register);
            Add(Addresses.RX_PW_P1, ReceivePayloadWidthPipe1Register);
            Add(Addresses.RX_PW_P2, ReceivePayloadWidthPipe2Register);
            Add(Addresses.RX_PW_P3, ReceivePayloadWidthPipe3Register);
            Add(Addresses.RX_PW_P4, ReceivePayloadWidthPipe4Register);
            Add(Addresses.RX_PW_P5, ReceivePayloadWidthPipe5Register);
            Add(Addresses.FIFO_STATUS, FifoStatusRegister);
            Add(Addresses.DYNPD, DynamicPayloadLengthRegister);
            Add(Addresses.FEATURE, FeatureRegister);
        }

        public void LoadAll()
        {
            foreach (RegisterBase register in Values)
                register.Load();
        }

        public void SaveAll()
        {
            foreach (RegisterBase register in Values)
                register.Save();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Register Values:\r\n");
            foreach (RegisterBase register in Values)
            {
                try
                {
                    sb.AppendFormat("{0}\r\n\t{1}\r\n\r\n", register.Name, register);
                }
                catch (Exception ex)
                {
                    sb.AppendFormat("{0}\r\n\tError:{1}\r\n\r\n", register.Name, ex.Message);
                }
            }

            return sb.ToString();
        }
    }
}
