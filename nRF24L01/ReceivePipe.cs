using System;

namespace Windows.Devices.Radios.nRF24L01P
{
    public class ReceivePipe
    {
        private readonly Registers.RegisterCollection _registers;
        private readonly Radio _radio;
        public int PipeId { get; private set; }
        public ReceivePipe(Radio radio, int pipeId)
        {
            _radio = radio;
            PipeId = pipeId;
            _registers = _radio.Configuration.Registers;
        }

        public byte[] Address
        {
            get
            {

                switch (PipeId)
                {
                    case 0:
                        return _registers.ReceiveAddressPipe0Register;
                    case 1:
                        return _registers.ReceiveAddressPipe1Register;
                    case 2:
                        return BitConverter.GetBytes(_registers.ReceiveAddressPipe2Register);
                    case 3:
                        return BitConverter.GetBytes(_registers.ReceiveAddressPipe3Register);
                    case 4:
                        return BitConverter.GetBytes(_registers.ReceiveAddressPipe4Register);
                    case 5:
                        return BitConverter.GetBytes(_registers.ReceiveAddressPipe5Register);
                    default:
                        throw new InvalidOperationException("Cannot get address for data pipe, invalid ID number for this Pipe");
                }
            }
            set
            {
                int addressWidth = _radio.Configuration.AddressWidth;
                if (PipeId < 2 && value.Length < addressWidth)
                    throw new InvalidOperationException("Address length should equal or greater than device.Config.AddressWidth");
                else if (PipeId < 2 && value.Length > addressWidth)
                    Array.Resize(ref value, addressWidth);
                else if (PipeId > 1 && value.Length != 1)
                    throw new InvalidOperationException("Address length should be 1 byte for receive pipes 2 to 5");
                switch (PipeId)
                {
                    case 0:
                        _registers.ReceiveAddressPipe0Register.Load(value);
                        _registers.ReceiveAddressPipe0Register.Save();
                        break;
                    case 1:
                        _registers.ReceiveAddressPipe1Register.Load(value);
                        _registers.ReceiveAddressPipe1Register.Save();
                        break;
                    case 2:
                        _registers.ReceiveAddressPipe2Register.Load(value);
                        _registers.ReceiveAddressPipe2Register.Save();
                        break;
                    case 3:
                        _registers.ReceiveAddressPipe3Register.Load(value);
                        _registers.ReceiveAddressPipe3Register.Save();
                        break;
                    case 4:
                        _registers.ReceiveAddressPipe4Register.Load(value);
                        _registers.ReceiveAddressPipe4Register.Save();
                        break;
                    case 5:
                        _registers.ReceiveAddressPipe5Register.Load(value);
                        _registers.ReceiveAddressPipe5Register.Save();
                        break;
                    default:
                        throw new InvalidOperationException("Cannot set address for data pipe, invalid ID number for this Pipe");
                }
            }
        }

        public bool Enabled
        {
            get
            {
                switch (PipeId)
                {
                    case 0:
                        return _registers.EnableReceiveAddressRegister.ERX_P0;
                    case 1:
                        return _registers.EnableReceiveAddressRegister.ERX_P1;
                    case 2:
                        return _registers.EnableReceiveAddressRegister.ERX_P2;
                    case 3:
                        return _registers.EnableReceiveAddressRegister.ERX_P3;
                    case 4:
                        return _registers.EnableReceiveAddressRegister.ERX_P4;
                    case 5:
                        return _registers.EnableReceiveAddressRegister.ERX_P5;
                    default:
                        throw new InvalidOperationException("Cannot get register value, invalid ID number for this Pipe");
                }

            }
            set
            {
                switch (PipeId)
                {
                    case 0:
                        _registers.EnableReceiveAddressRegister.ERX_P0 = value;
                        _registers.EnableReceiveAddressRegister.Save();
                        break;
                    case 1:
                        _registers.EnableReceiveAddressRegister.ERX_P1 = value;
                        _registers.EnableReceiveAddressRegister.Save();
                        break;
                    case 2:
                        _registers.EnableReceiveAddressRegister.ERX_P2 = value;
                        _registers.EnableReceiveAddressRegister.Save();
                        break;
                    case 3:
                        _registers.EnableReceiveAddressRegister.ERX_P3 = value;
                        _registers.EnableReceiveAddressRegister.Save();
                        break;
                    case 4:
                        _registers.EnableReceiveAddressRegister.ERX_P4 = value;
                        _registers.EnableReceiveAddressRegister.Save();
                        break;
                    case 5:
                        _registers.EnableReceiveAddressRegister.ERX_P5 = value;
                        _registers.EnableReceiveAddressRegister.Save();
                        break;
                    default:
                        throw new InvalidOperationException("Cannot set register value, invalid ID number for this Pipe");
                }

            }
        }

        public bool AutoAcknowledgementEnabled
        {
            get
            {
                switch (PipeId)
                {
                    case 0:
                        return _registers.EnableAutoAcknowledgementRegister.ENAA_P0;
                    case 1:
                        return _registers.EnableAutoAcknowledgementRegister.ENAA_P1;
                    case 2:
                        return _registers.EnableAutoAcknowledgementRegister.ENAA_P2;
                    case 3:
                        return _registers.EnableAutoAcknowledgementRegister.ENAA_P3;
                    case 4:
                        return _registers.EnableAutoAcknowledgementRegister.ENAA_P4;
                    case 5:
                        return _registers.EnableAutoAcknowledgementRegister.ENAA_P5;
                    default:
                        throw new InvalidOperationException("Cannot get DynamicPayloadEnabled for data pipe, invalid ID number for this Pipe");
                }
            }
            set
            {
                switch (PipeId)
                {
                    case 0:
                        _registers.EnableAutoAcknowledgementRegister.ENAA_P0 = value;
                        _registers.EnableAutoAcknowledgementRegister.Save();
                        break;
                    case 1:
                        _registers.EnableAutoAcknowledgementRegister.ENAA_P1 = value;
                        _registers.EnableAutoAcknowledgementRegister.Save();
                        break;
                    case 2:
                        _registers.EnableAutoAcknowledgementRegister.ENAA_P2 = value;
                        _registers.EnableAutoAcknowledgementRegister.Save();
                        break;
                    case 3:
                        _registers.EnableAutoAcknowledgementRegister.ENAA_P3 = value;
                        _registers.EnableAutoAcknowledgementRegister.Save();
                        break;
                    case 4:
                        _registers.EnableAutoAcknowledgementRegister.ENAA_P4 = value;
                        _registers.EnableAutoAcknowledgementRegister.Save();
                        break;
                    case 5:
                        _registers.EnableAutoAcknowledgementRegister.ENAA_P5 = value;
                        _registers.EnableAutoAcknowledgementRegister.Save();
                        break;
                    default:
                        throw new InvalidOperationException("Cannot set DynamicPayloadEnabled for data pipe, invalid ID number for this Pipe");
                }
            }
        }

        public bool DynamicPayloadLengthEnabled
        {
            get
            {
                switch (PipeId)
                {
                    case 0:
                        return _registers.DynamicPayloadLengthRegister.DPL_P0;
                    case 1:
                        return _registers.DynamicPayloadLengthRegister.DPL_P1;
                    case 2:
                        return _registers.DynamicPayloadLengthRegister.DPL_P2;
                    case 3:
                        return _registers.DynamicPayloadLengthRegister.DPL_P3;
                    case 4:
                        return _registers.DynamicPayloadLengthRegister.DPL_P4;
                    case 5:
                        return _registers.DynamicPayloadLengthRegister.DPL_P5;
                    default:
                        throw new InvalidOperationException("Cannot get DynamicPayloadLengthEnabled for data pipe, invalid ID number for this Pipe");
                }
            }
            set
            {
                if (value && !_radio.Configuration.DynamicPayloadLengthEnabled)
                {
                    throw new InvalidOperationException("please enable Config.DynamicPayloadLengthEnabled before you enable this feature on data pipe");
                }
                if (value && !AutoAcknowledgementEnabled)
                {
                    throw new InvalidOperationException("please enable AutoACK of current data pipe before you can enable this feature on current data pipe");
                }
                switch (PipeId)
                {
                    case 0:
                        _registers.DynamicPayloadLengthRegister.DPL_P0 = value;
                        _registers.DynamicPayloadLengthRegister.Save();
                        break;
                    case 1:
                        _registers.DynamicPayloadLengthRegister.DPL_P1 = value;
                        _registers.DynamicPayloadLengthRegister.Save();
                        break;
                    case 2:
                        _registers.DynamicPayloadLengthRegister.DPL_P2 = value;
                        _registers.DynamicPayloadLengthRegister.Save();
                        break;
                    case 3:
                        _registers.DynamicPayloadLengthRegister.DPL_P3 = value;
                        _registers.DynamicPayloadLengthRegister.Save();
                        break;
                    case 4:
                        _registers.DynamicPayloadLengthRegister.DPL_P4 = value;
                        _registers.DynamicPayloadLengthRegister.Save();
                        break;
                    case 5:
                        _registers.DynamicPayloadLengthRegister.DPL_P5 = value;
                        _registers.DynamicPayloadLengthRegister.Save();
                        break;
                    default:
                        throw new InvalidOperationException("Cannot set DynamicPayloadLengthEnabled for data pipe, invalid ID number for this Pipe");
                }
            }
        }

        public byte PayloadWidth
        {
            get
            {
                switch (PipeId)
                {
                    case 0:
                        return _registers.ReceivePayloadWidthPipe0Register.RX_PW_P0;
                    case 1:
                        return _registers.ReceivePayloadWidthPipe1Register.RX_PW_P1;
                    case 2:
                        return _registers.ReceivePayloadWidthPipe2Register.RX_PW_P2;
                    case 3:
                        return _registers.ReceivePayloadWidthPipe3Register.RX_PW_P3;
                    case 4:
                        return _registers.ReceivePayloadWidthPipe4Register.RX_PW_P4;
                    case 5:
                        return _registers.ReceivePayloadWidthPipe5Register.RX_PW_P5;
                    default:
                        throw new InvalidOperationException("Cannot get PayloadWidth for data pipe, invalid ID number for this Pipe");
                }
            }
            set
            {
                switch (PipeId)
                {
                    case 0:
                        _registers.ReceivePayloadWidthPipe0Register.RX_PW_P0 = value;
                        _registers.ReceivePayloadWidthPipe0Register.Save();
                        break;
                    case 1:
                        _registers.ReceivePayloadWidthPipe1Register.RX_PW_P1 = value;
                        _registers.ReceivePayloadWidthPipe1Register.Save();
                        break;
                    case 2:
                        _registers.ReceivePayloadWidthPipe2Register.RX_PW_P2 = value;
                        _registers.ReceivePayloadWidthPipe2Register.Save();
                        break;
                    case 3:
                        _registers.ReceivePayloadWidthPipe3Register.RX_PW_P3 = value;
                        _registers.ReceivePayloadWidthPipe3Register.Save();
                        break;
                    case 4:
                        _registers.ReceivePayloadWidthPipe4Register.RX_PW_P4 = value;
                        _registers.ReceivePayloadWidthPipe4Register.Save();
                        break;
                    case 5:
                        _registers.ReceivePayloadWidthPipe5Register.RX_PW_P5 = value;
                        _registers.ReceivePayloadWidthPipe5Register.Save();
                        break;
                    default:
                        throw new InvalidOperationException("Cannot set PayloadLength width for data pipe, invalid ID number for this Pipe");
                }
            }
        }

        public byte BytesToRead => _radio.Transfer(Commands.R_RX_PL_WID | Commands.EMPTY_ADDRESS);
    }
}
