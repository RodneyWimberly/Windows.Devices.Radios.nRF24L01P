using System.Linq;
using System.Text;
using Windows.Devices.Radios.nRF24L01P.Enums;
using Windows.Devices.Radios.nRF24L01P.Interfaces;
using Windows.Devices.Radios.nRF24L01P.Registers;

namespace Windows.Devices.Radios.nRF24L01P
{
    public class Diagnostics
    {
        private readonly IRadio _radio;

        public Diagnostics(IRadio radio)
        {
            _radio = radio;
        }

        public byte ReadRegister(byte register)
        {
            return _radio.CommandProcessor.ExecuteCommand(DeviceCommands.R_REGISTER, register, new byte[1])[0];
        }

        public byte ReadRegister(byte register, ref byte[] buffer, int length = 0)
        {
            buffer = _radio.CommandProcessor.ExecuteCommand(DeviceCommands.R_REGISTER, register, buffer);
            return _radio.Configuration.Registers.StatusRegister;
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
            string extraTab = string.Empty;
            if (name.Length < 8)
                extraTab += "\t\t";
            else if (name.Length < 12)
                extraTab += "\t";
            string registerValue = string.Format("{0}{1}\t =", name, extraTab);
            for (int index = 0; index < quantity; index++)
            {
                byte[] buffer = new byte[5];
                ReadRegister(register++, ref buffer, buffer.Length);
                registerValue += " 0x"; // + BitConverter.ToString(buffer).Replace("-", string.Empty);
                registerValue = buffer.Aggregate(registerValue, (current, part) => current + part.ToString("X"));
            }

            return registerValue;
        }

        private string GetByteRegister(string name, byte register, int quantity)
        {
            string extraTab = string.Empty;
            if (name.Length < 8)
                extraTab += "\t\t";
            else if (name.Length < 12)
                extraTab += "\t";
            string registerValue = string.Format("{0}{1}\t =", name, extraTab);
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
            sb.AppendFormat("STATUS\t\t\t = 0x{0} RX_DR={1} TX_DS={2} MAX_RT={3} RX_P_NO={4} TX_FULL={5}\r\n",
                status.ToString("X"),
                (status & Utilities.BitValue(Properties.RX_DR)),
                (status & Utilities.BitValue(Properties.TX_DS)),
                (status & Utilities.BitValue(Properties.MAX_RT)),
                (status << (4) >> (5)),
                (status & Utilities.BitValue(Properties.TX_FULL)));

            sb.AppendLine(GetAddressRegister("RX_ADDR_P0-1", RegisterAddresses.RX_ADDR_P0, 2));
            sb.AppendLine(GetByteRegister("RX_ADDR_P2-5", RegisterAddresses.RX_ADDR_P2, 4));
            sb.AppendLine(GetAddressRegister("TX_ADDR", RegisterAddresses.TX_ADDR, 1));

            sb.AppendLine(GetByteRegister("RX_PW_P0-6", RegisterAddresses.RX_PW_P0, 6));
            sb.AppendLine(GetByteRegister("EN_AA", RegisterAddresses.EN_AA, 1));
            sb.AppendLine(GetByteRegister("EN_RXADDR", RegisterAddresses.EN_RXADDR, 1));
            sb.AppendLine(GetByteRegister("RF_CH", RegisterAddresses.RF_CH, 1));
            sb.AppendLine(GetByteRegister("RF_SETUP", RegisterAddresses.RF_SETUP, 1));
            sb.AppendLine(GetByteRegister("CONFIG", RegisterAddresses.CONFIG, 1));
            sb.AppendLine(GetByteRegister("DYNPD/FEATURE", RegisterAddresses.DYNPD, 2));

            sb.AppendLine("Data Rate\t\t = " + Constants.DataRates[(int)_radio.Configuration.DataRate]);
            sb.AppendLine("Model\t\t\t = " + _radio.Name);
            sb.AppendLine("CRC Enabled\t\t = " + _radio.Configuration.CrcEnabled);
            sb.AppendLine("CRC Encoding\t = " + Constants.CrcEncodingSchemes[(int)_radio.Configuration.CrcEncodingScheme]);
            sb.AppendLine("PA Power\t\t = " + Constants.PowerLevels[(int)_radio.Configuration.PowerLevel]);

            return sb.ToString();
        }

    }
}
