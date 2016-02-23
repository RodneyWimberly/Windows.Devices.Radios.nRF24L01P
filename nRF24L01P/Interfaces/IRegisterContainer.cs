using Windows.Devices.Radios.nRF24L01P.Registers;

namespace Windows.Devices.Radios.nRF24L01P.Interfaces
{
    public interface IRegisterContainer
    {
        ConfigurationRegister ConfigurationRegister { get; }
        EnableAutoAcknowledgementRegister EnableAutoAcknowledgementRegister { get; }
        EnableReceiveAddressRegister EnableReceiveAddressRegister { get; }
        SetupAddressWidthRegister AddressWidthRegister { get; }
        SetupRetransmissionRegister SetupRetransmissionRegister { get; }
        RfChannelRegister RfChannelRegister { get; }
        RfSetupRegister RfSetupRegister { get; }
        StatusRegister StatusRegister { get; }
        ObserveTransmitRegister ObserveTransmitRegister { get; }
        ReceivedPowerDetectorRegister ReceivedPowerDetectorRegister { get; }
        AddressPipeRegister TransmitAddressRegister { get; }
        FifoStatusRegister FifoStatusRegister { get; }
        DynamicPayloadLengthRegister DynamicPayloadLengthRegister { get; }
        FeatureRegister FeatureRegister { get; }
        IRegisterCollection<IRegister> AllRegisters { get; }
        IRegisterCollection<AddressPipeRegister> ReceiveAddressPipeRegisters { get; }
        IRegisterCollection<ReceivePayloadWidthPipeRegister> ReceivePayloadWidthPipeRegisters { get; }
        void LoadRegisters();
        void ResetRegistersToDefault();
        void SaveRegisters(bool force = false);
        string ToString();
    }
}