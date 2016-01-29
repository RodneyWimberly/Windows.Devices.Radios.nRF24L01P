using System;
using System.Collections.Generic;
using System.Text;

namespace Windows.Devices.Radios.nRF24L01.Registers
{
    public class RegisterCollection : Dictionary<byte, RegisterBase>
    {
        private Radio _radio;

        public ConfigRegister ConfigRegister { get; private set; }
        public AutoAckRegister AutoAckRegister { get; private set; }
        public ReceivePipeRegister ReceivePipeRegister { get; private set; }
        public AddressWidthRegister AddressWidthRegister { get; private set; }
        public SetupRetryRegister SetupRetryRegister { get; private set; }
        public ChannelRegister ChannelRegister { get; private set; }
        public SetupRegister SetupRegister { get; private set; }
        public StatusRegister StatusRegister { get; private set; }
        public ObserveTransmitRegister ObserveTransmitRegister { get; private set; }
        public ReceivedPowerDetectorRegister ReceivedPowerDetectorRegister { get; private set; }
        public ReceivePipe0AddressRegister ReceivePipe0AddressRegister { get; private set; }
        public ReceivePipe1AddressRegister ReceivePipe1AddressRegister { get; private set; }
        public ReceivePipe2AddressRegister ReceivePipe2AddressRegister { get; private set; }
        public ReceivePipe3AddressRegister ReceivePipe3AddressRegister { get; private set; }
        public ReceivePipe4AddressRegister ReceivePipe4AddressRegister { get; private set; }
        public ReceivePipe5AddressRegister ReceivePipe5AddressRegister { get; private set; }
        public TransmitAddressRegister TransmitAddressRegister { get; private set; }
        public ReceivePipe0PayloadWidthRegister ReceivePipe0PayloadWidthRegister { get; private set; }
        public ReceivePipe1PayloadWidthRegister ReceivePipe1PayloadWidthRegister { get; private set; }
        public ReceivePipe2PayloadWidthRegister ReceivePipe2PayloadWidthRegister { get; private set; }
        public ReceivePipe3PayloadWidthRegister ReceivePipe3PayloadWidthRegister { get; private set; }
        public ReceivePipe4PayloadWidthRegister ReceivePipe4PayloadWidthRegister { get; private set; }
        public ReceivePipe5PayloadWidthRegister ReceivePipe5PayloadWidthRegister { get; private set; }
        public FifoStatusRegister FifoStatusRegister { get; private set; }
        public DynamicPayloadRegister DynamicPayloadRegister { get; private set; }
        public FeatureRegister FeatureRegister { get; private set; }

        public RegisterCollection(Radio radio)
        {
            _radio = radio;
            ConfigRegister = new ConfigRegister(radio);
            AutoAckRegister = new AutoAckRegister(radio);
            ReceivePipeRegister = new ReceivePipeRegister(radio);
            AddressWidthRegister = new AddressWidthRegister(radio);
            SetupRetryRegister = new SetupRetryRegister(radio);
            ChannelRegister = new ChannelRegister(radio);
            SetupRegister = new SetupRegister(radio);
            StatusRegister = new StatusRegister(radio);
            ObserveTransmitRegister = new ObserveTransmitRegister(radio);
            ReceivedPowerDetectorRegister = new ReceivedPowerDetectorRegister(radio);
            ReceivePipe0AddressRegister = new ReceivePipe0AddressRegister(radio);
            ReceivePipe1AddressRegister = new ReceivePipe1AddressRegister(radio);
            ReceivePipe2AddressRegister = new ReceivePipe2AddressRegister(radio);
            ReceivePipe3AddressRegister = new ReceivePipe3AddressRegister(radio);
            ReceivePipe4AddressRegister = new ReceivePipe4AddressRegister(radio);
            ReceivePipe5AddressRegister = new ReceivePipe5AddressRegister(radio);
            TransmitAddressRegister = new TransmitAddressRegister(radio);
            ReceivePipe0PayloadWidthRegister = new ReceivePipe0PayloadWidthRegister(radio);
            ReceivePipe1PayloadWidthRegister = new ReceivePipe1PayloadWidthRegister(radio);
            ReceivePipe2PayloadWidthRegister = new ReceivePipe2PayloadWidthRegister(radio);
            ReceivePipe3PayloadWidthRegister = new ReceivePipe3PayloadWidthRegister(radio);
            ReceivePipe4PayloadWidthRegister = new ReceivePipe4PayloadWidthRegister(radio);
            ReceivePipe5PayloadWidthRegister = new ReceivePipe5PayloadWidthRegister(radio);
            FifoStatusRegister = new FifoStatusRegister(radio);
            DynamicPayloadRegister = new DynamicPayloadRegister(radio);
            FeatureRegister = new FeatureRegister(radio);

            Add(Constants.CONFIG, ConfigRegister);
            Add(Constants.EN_AA, AutoAckRegister);
            Add(Constants.EN_RXADDR, ReceivePipeRegister);
            Add(Constants.SETUP_AW, AddressWidthRegister);
            Add(Constants.SETUP_RETR, SetupRetryRegister);
            Add(Constants.RF_CH, ChannelRegister);
            Add(Constants.RF_SETUP, SetupRegister);
            Add(Constants.STATUS, StatusRegister);
            Add(Constants.OBSERVE_TX, ObserveTransmitRegister);
            Add(Constants.RPD, ReceivedPowerDetectorRegister);
            Add(Constants.RX_ADDR_P0, ReceivePipe0AddressRegister);
            Add(Constants.RX_ADDR_P1, ReceivePipe1AddressRegister);
            Add(Constants.RX_ADDR_P2, ReceivePipe2AddressRegister);
            Add(Constants.RX_ADDR_P3, ReceivePipe3AddressRegister);
            Add(Constants.RX_ADDR_P4, ReceivePipe4AddressRegister);
            Add(Constants.RX_ADDR_P5, ReceivePipe5AddressRegister);
            Add(Constants.TX_ADDR, TransmitAddressRegister);
            Add(Constants.RX_PW_P0, ReceivePipe0PayloadWidthRegister);
            Add(Constants.RX_PW_P1, ReceivePipe1PayloadWidthRegister);
            Add(Constants.RX_PW_P2, ReceivePipe2PayloadWidthRegister);
            Add(Constants.RX_PW_P3, ReceivePipe3PayloadWidthRegister);
            Add(Constants.RX_PW_P4, ReceivePipe4PayloadWidthRegister);
            Add(Constants.RX_PW_P5, ReceivePipe5PayloadWidthRegister);
            Add(Constants.FIFO_STATUS, FifoStatusRegister);
            Add(Constants.DYNPD, DynamicPayloadRegister);
            Add(Constants.FEATURE, FeatureRegister);
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
