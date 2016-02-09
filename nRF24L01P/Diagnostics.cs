using System;
using System.Text;
using Windows.Devices.Radios.nRF24L01P.Registers;

namespace Windows.Devices.Radios.nRF24L01P
{
    public class Diagnostics
    {
        private readonly Radio _radio;

        public Diagnostics(Radio radio)
        {
            _radio = radio;
        }

        public byte ReadRegister(byte register)
        {
            byte[] request = new byte[2],
                response = new byte[1];

            request[0] = (byte)(Commands.R_REGISTER | (Commands.REGISTER_MASK & register));
            response = _radio.Transfer(request);
            return response[0];
        }

        public byte ReadRegister(byte register, ref byte[] buffer, int length = 0)
        {
            byte[] request = new byte[length + 1];
            request[0] = (byte)(Commands.R_REGISTER | (Commands.REGISTER_MASK & register));
            Array.Copy(buffer, 0, request, 1, length);
            var response = _radio.Transfer(request);
            buffer = new byte[response.Length - 1];
            Array.Copy(response, 0, buffer, 0, buffer.Length);
            return response[0];
        }

        private string FormatObserveTx(byte value)
        {
            return string.Format("OBSERVE_TX={0:X2}: POLS_CNT={1} ARC_CNT={2}",
                value,
                (value >> Properties.PLOS_CNT) & 0xF,
                (value >> Properties.ARC_CNT) & 0xF);
        }

        private string GetAddressRegister(string name, byte register, int quantity)
        {
            string registerValue = name + " =";
            for (int index = 0; index < quantity; index++)
            {
                byte[] buffer = new byte[5];
                ReadRegister(register++, ref buffer, buffer.Length);
                registerValue += " 0x" + BitConverter.ToString(buffer).Replace("-", string.Empty);
            }

            return registerValue;
        }

        private string GetByteRegister(string name, byte register, int quantity)
        {
            string registerValue = name + " =";
            for (int index = 0; index < quantity; index++)
            {
                registerValue += " 0x" + ReadRegister(register++).ToString("X");
            }

            return registerValue;
        }

        public string GetDetails()
        {
            StringBuilder sb = new StringBuilder();

            byte status = _radio.Configuration.Registers.StatusRegister;
            sb.AppendFormat("STATUS = 0x{0} RX_DR={1} TX_DS={2} MAX_RT={3} RX_P_NO={4} TX_FULL={5}\r\n",
                status.ToString("X"),
                (status & Utilities.BitValue(Properties.RX_DR)),
                (status & Utilities.BitValue(Properties.TX_DS)),
                (status & Utilities.BitValue(Properties.MAX_RT)),
                (status << (4) >> (5)),
                (status & Utilities.BitValue(Properties.TX_FULL)));

            sb.AppendLine(GetAddressRegister("RX_ADDR_P0-1", Addresses.RX_ADDR_P0, 2));
            sb.AppendLine(GetByteRegister("RX_ADDR_P2-5", Addresses.RX_ADDR_P2, 4));
            sb.AppendLine(GetAddressRegister("TX_ADDR", Addresses.TX_ADDR, 1));

            sb.AppendLine(GetByteRegister("RX_PW_P0-6", Addresses.RX_PW_P0, 6));
            sb.AppendLine(GetByteRegister("EN_AA", Addresses.EN_AA, 1));
            sb.AppendLine(GetByteRegister("EN_RXADDR", Addresses.EN_RXADDR, 1));
            sb.AppendLine(GetByteRegister("RF_CH", Addresses.RF_CH, 1));
            sb.AppendLine(GetByteRegister("RF_SETUP", Addresses.RF_SETUP, 1));
            sb.AppendLine(GetByteRegister("CONFIG", Addresses.CONFIG, 1));
            sb.AppendLine(GetByteRegister("DYNPD/FEATURE", Addresses.DYNPD, 2));

            sb.AppendLine("Data Rate = " + Constants.DataRates[(int)_radio.Configuration.DataRate]);
            sb.AppendLine("Model = " + _radio.Name);
            sb.AppendLine("CRC Enabled = " + _radio.Configuration.CrcEnabled);
            sb.AppendLine("CRC Encoding Scheme = " + Constants.CrcEncodingSchemes[(int)_radio.Configuration.CrcEncodingScheme]);
            sb.AppendLine("PA Power = " + Constants.PowerLevels[(int)_radio.Configuration.PowerLevel]);

            sb.AppendFormat("\r\n{0}\r\n", _radio.Configuration.Registers);
            return sb.ToString();
        }

    }
}
