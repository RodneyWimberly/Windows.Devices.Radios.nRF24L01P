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
        private readonly IRadioConfiguration _configuration;
        private readonly ICommandProcessor _commandProcessor;

        public Diagnostics(IRadio radio, IRadioConfiguration configuration, ICommandProcessor commandProcessor)
        {
            _radio = radio;
            _configuration = configuration;
            _commandProcessor = commandProcessor;
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
                buffer = _commandProcessor.ExecuteCommand(DeviceCommands.R_REGISTER, register++, buffer);
                registerValue += " 0x";
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
                registerValue += " 0x" + _commandProcessor.ExecuteCommand(DeviceCommands.R_REGISTER, register++, new byte[1])[0].ToString("X");

            return registerValue;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            byte status = _configuration.Registers.StatusRegister;
            sb.AppendFormat("STATUS\t\t\t = 0x{0} RX_DR={1} TX_DS={2} MAX_RT={3} RX_P_NO={4} TX_FULL={5}\r\n",
                status.ToString("X"),
                (status & (byte)PropertyMasks.RX_DR),
                (status & (byte)PropertyMasks.TX_DS),
                (status & (byte)PropertyMasks.MAX_RT),
                (status & (byte)PropertyMasks.RX_P_NO),
                (status & (byte)PropertyMasks.TX_FULL));
            //(status << (4) >> (5)),

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

            sb.AppendLine("Data Rate\t\t = " + _configuration.DataRate.GetName());
            sb.AppendLine("Model\t\t\t = " + _configuration.RadioModel.GetName());
            sb.AppendLine("CRC Enabled\t\t = " + _configuration.CrcEnabled);
            sb.AppendLine("CRC Encoding\t = " + _configuration.CrcEncodingScheme.GetName());
            sb.AppendLine("PA Power\t\t = " + _configuration.PowerLevel.GetName());
            sb.AppendLine("Retransmit Delay = " + _configuration.AutoRetransmitDelay.GetName());
            sb.AppendLine("Device Status\t = " + _radio.Status.GetName());
            sb.AppendLine("Transmit FIFO\t = " + _radio.TransmitPipe.FifoStatus.GetName());
            sb.AppendLine("Receive FIFO\t = " + _radio.ReceivePipes[0].FifoStatus.GetName());
            sb.AppendLine("Power Detector\t = " + _radio.Configuration.Registers.ReceivedPowerDetectorRegister.ReceivedPowerDetector);
            return sb.ToString();
        }
    }
}
