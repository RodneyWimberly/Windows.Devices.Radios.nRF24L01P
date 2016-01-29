using System;

namespace Windows.Devices.Radios.nRF24L01
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
                        return _registers.ReceivePipe0AddressRegister.RX_ADDR_P0;
                    case 1:
                        return _registers.ReceivePipe1AddressRegister.RX_ADDR_P1;
                    case 2:
                        return BitConverter.GetBytes(_registers.ReceivePipe2AddressRegister.RX_ADDR_P2);
                    case 3:
                        return BitConverter.GetBytes(_registers.ReceivePipe3AddressRegister.RX_ADDR_P3);
                    case 4:
                        return BitConverter.GetBytes(_registers.ReceivePipe4AddressRegister.RX_ADDR_P4);
                    case 5:
                        return BitConverter.GetBytes(_registers.ReceivePipe5AddressRegister.RX_ADDR_P5);
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
                        _registers.ReceivePipe0AddressRegister.RX_ADDR_P0 = value;
                        _registers.ReceivePipe0AddressRegister.Save();
                        break;
                    case 1:
                        _registers.ReceivePipe1AddressRegister.RX_ADDR_P1 = value;
                        _registers.ReceivePipe1AddressRegister.Save();
                        break;
                    case 2:
                        _registers.ReceivePipe2AddressRegister.RX_ADDR_P2 = value[0];
                        _registers.ReceivePipe2AddressRegister.Save();
                        break;
                    case 3:
                        _registers.ReceivePipe3AddressRegister.RX_ADDR_P3 = value[0];
                        _registers.ReceivePipe3AddressRegister.Save();
                        break;
                    case 4:
                        _registers.ReceivePipe4AddressRegister.RX_ADDR_P4 = value[0];
                        _registers.ReceivePipe4AddressRegister.Save();
                        break;
                    case 5:
                        _registers.ReceivePipe5AddressRegister.RX_ADDR_P5 = value[0];
                        _registers.ReceivePipe5AddressRegister.Save();
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
                        return _registers.ReceivePipeRegister.ERX_P0;
                    case 1:
                        return _registers.ReceivePipeRegister.ERX_P1;
                    case 2:
                        return _registers.ReceivePipeRegister.ERX_P2;
                    case 3:
                        return _registers.ReceivePipeRegister.ERX_P3;
                    case 4:
                        return _registers.ReceivePipeRegister.ERX_P4;
                    case 5:
                        return _registers.ReceivePipeRegister.ERX_P5;
                    default:
                        throw new InvalidOperationException("Cannot get register value, invalid ID number for this Pipe");
                }

            }
            set
            {
                switch (PipeId)
                {
                    case 0:
                        _registers.ReceivePipeRegister.ERX_P0 = value;
                        _registers.ReceivePipeRegister.Save();
                        break;
                    case 1:
                        _registers.ReceivePipeRegister.ERX_P1 = value;
                        _registers.ReceivePipeRegister.Save();
                        break;
                    case 2:
                        _registers.ReceivePipeRegister.ERX_P2 = value;
                        _registers.ReceivePipeRegister.Save();
                        break;
                    case 3:
                        _registers.ReceivePipeRegister.ERX_P3 = value;
                        _registers.ReceivePipeRegister.Save();
                        break;
                    case 4:
                        _registers.ReceivePipeRegister.ERX_P4 = value;
                        _registers.ReceivePipeRegister.Save();
                        break;
                    case 5:
                        _registers.ReceivePipeRegister.ERX_P5 = value;
                        _registers.ReceivePipeRegister.Save();
                        break;
                    default:
                        throw new InvalidOperationException("Cannot set register value, invalid ID number for this Pipe");
                }

            }
        }

        public bool AutoACKEnabled
        {
            get
            {
                switch (PipeId)
                {
                    case 0:
                        return _registers.AutoAckRegister.ENAA_P0;
                    case 1:
                        return _registers.AutoAckRegister.ENAA_P1;
                    case 2:
                        return _registers.AutoAckRegister.ENAA_P2;
                    case 3:
                        return _registers.AutoAckRegister.ENAA_P3;
                    case 4:
                        return _registers.AutoAckRegister.ENAA_P4;
                    case 5:
                        return _registers.AutoAckRegister.ENAA_P5;
                    default:
                        throw new InvalidOperationException("Cannot get DynamicPayloadEnabled for data pipe, invalid ID number for this Pipe");
                }
            }
            set
            {
                switch (PipeId)
                {
                    case 0:
                        _registers.AutoAckRegister.ENAA_P0 = value;
                        _registers.AutoAckRegister.Save();
                        break;
                    case 1:
                        _registers.AutoAckRegister.ENAA_P1 = value;
                        _registers.AutoAckRegister.Save();
                        break;
                    case 2:
                        _registers.AutoAckRegister.ENAA_P2 = value;
                        _registers.AutoAckRegister.Save();
                        break;
                    case 3:
                        _registers.AutoAckRegister.ENAA_P3 = value;
                        _registers.AutoAckRegister.Save();
                        break;
                    case 4:
                        _registers.AutoAckRegister.ENAA_P4 = value;
                        _registers.AutoAckRegister.Save();
                        break;
                    case 5:
                        _registers.AutoAckRegister.ENAA_P5 = value;
                        _registers.AutoAckRegister.Save();
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
                        return _registers.DynamicPayloadRegister.DPL_P0;
                    case 1:
                        return _registers.DynamicPayloadRegister.DPL_P1;
                    case 2:
                        return _registers.DynamicPayloadRegister.DPL_P2;
                    case 3:
                        return _registers.DynamicPayloadRegister.DPL_P3;
                    case 4:
                        return _registers.DynamicPayloadRegister.DPL_P4;
                    case 5:
                        return _registers.DynamicPayloadRegister.DPL_P5;
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
                if (value && !AutoACKEnabled)
                {
                    throw new InvalidOperationException("please enable AutoACK of current data pipe before you can enable this feature on current data pipe");
                }
                switch (PipeId)
                {
                    case 0:
                        _registers.DynamicPayloadRegister.DPL_P0 = value;
                        _registers.DynamicPayloadRegister.Save();
                        break;
                    case 1:
                        _registers.DynamicPayloadRegister.DPL_P1 = value;
                        _registers.DynamicPayloadRegister.Save();
                        break;
                    case 2:
                        _registers.DynamicPayloadRegister.DPL_P2 = value;
                        _registers.DynamicPayloadRegister.Save();
                        break;
                    case 3:
                        _registers.DynamicPayloadRegister.DPL_P3 = value;
                        _registers.DynamicPayloadRegister.Save();
                        break;
                    case 4:
                        _registers.DynamicPayloadRegister.DPL_P4 = value;
                        _registers.DynamicPayloadRegister.Save();
                        break;
                    case 5:
                        _registers.DynamicPayloadRegister.DPL_P5 = value;
                        _registers.DynamicPayloadRegister.Save();
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
                        return _registers.ReceivePipe0PayloadWidthRegister.RX_PW_P0;
                    case 1:
                        return _registers.ReceivePipe1PayloadWidthRegister.RX_PW_P1;
                    case 2:
                        return _registers.ReceivePipe2PayloadWidthRegister.RX_PW_P2;
                    case 3:
                        return _registers.ReceivePipe3PayloadWidthRegister.RX_PW_P3;
                    case 4:
                        return _registers.ReceivePipe4PayloadWidthRegister.RX_PW_P4;
                    case 5:
                        return _registers.ReceivePipe5PayloadWidthRegister.RX_PW_P5;
                    default:
                        throw new InvalidOperationException("Cannot get PayloadWidth for data pipe, invalid ID number for this Pipe");
                }
            }
            set
            {
                switch (PipeId)
                {
                    case 0:
                        _registers.ReceivePipe0PayloadWidthRegister.RX_PW_P0 = value;
                        _registers.ReceivePipe0PayloadWidthRegister.Save();
                        break;
                    case 1:
                        _registers.ReceivePipe1PayloadWidthRegister.RX_PW_P1 = value;
                        _registers.ReceivePipe1PayloadWidthRegister.Save();
                        break;
                    case 2:
                        _registers.ReceivePipe2PayloadWidthRegister.RX_PW_P2 = value;
                        _registers.ReceivePipe2PayloadWidthRegister.Save();
                        break;
                    case 3:
                        _registers.ReceivePipe3PayloadWidthRegister.RX_PW_P3 = value;
                        _registers.ReceivePipe3PayloadWidthRegister.Save();
                        break;
                    case 4:
                        _registers.ReceivePipe4PayloadWidthRegister.RX_PW_P4 = value;
                        _registers.ReceivePipe4PayloadWidthRegister.Save();
                        break;
                    case 5:
                        _registers.ReceivePipe5PayloadWidthRegister.RX_PW_P5 = value;
                        _registers.ReceivePipe5PayloadWidthRegister.Save();
                        break;
                    default:
                        throw new InvalidOperationException("Cannot set PayloadLength width for data pipe, invalid ID number for this Pipe");
                }
            }
        }

        public byte BytesToRead => _radio.Transfer(Constants.R_RX_PL_WID | Constants.EMPTY_ADDRESS);
    }
}
