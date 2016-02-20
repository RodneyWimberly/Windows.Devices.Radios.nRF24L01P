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
        private readonly IConfiguration _configuration;
        private readonly ICommandProcessor _commandProcessor;
        private readonly IRegisterContainer _registerContainer;

        public Diagnostics(IRadio radio, IConfiguration configuration, ICommandProcessor commandProcessor, IRegisterContainer registerContainer)
        {
            _radio = radio;
            _configuration = configuration;
            _commandProcessor = commandProcessor;
            _registerContainer = registerContainer;
        }

        private string GetAddressRegister(string name, byte register, int quantity)
        {
            string extraTab = string.Empty;
            if (name.Length < 8)
                extraTab += "\t\t";
            else if (name.Length < 12)
                extraTab += "\t";
            string registerValue = string.Format("{0}{1}\t\t\t =", name, extraTab);
            for (int index = 0; index < quantity; index++)
            {
                byte[] buffer = new byte[5];
                buffer = _commandProcessor.ExecuteCommand(DeviceCommands.R_REGISTER, register++, buffer);
                registerValue += " 0x";
                registerValue = buffer.Aggregate(registerValue, (current, part) => current + part.ToString("X").PadLeft(2, '0'));
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
            string registerValue = string.Format("{0}{1}\t\t\t =", name, extraTab);
            for (int index = 0; index < quantity; index++)
                registerValue += " 0x" + _commandProcessor.ExecuteCommand(DeviceCommands.R_REGISTER, register++, new byte[1])[0].ToString("X").PadLeft(2, '0');

            return registerValue;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            
            StatusRegister statusRegister = _registerContainer.StatusRegister;
            statusRegister.Load();
            byte status = statusRegister;
            sb.AppendFormat("STATUS\t\t\t\t\t = 0x{0} \r\n" +
                            " ReceiveDataReady\t\t = {1} \r\n" +
                            " TransmitDataSent\t\t = {2} \r\n" +
                            " MaximunTransmitRetries\t = {3} \r\n" +
                            " ReceiveDataPipeNumber\t = {4} \r\n" +
                            " TransmitFifoFull\t\t = {5}\r\n",
                status.ToString("X").PadLeft(2, '0'),
                statusRegister.ReceiveDataReady,
                statusRegister.TransmitDataSent,
                statusRegister.MaximunTransmitRetries,
                statusRegister.ReceiveDataPipeNumber,
                statusRegister.TransmitFifoFull);

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

            sb.AppendLine("Data Rate\t\t\t\t = " + _configuration.DataRate.GetName());
            sb.AppendLine("Model\t\t\t\t\t = " + _configuration.RadioModel.GetName());
            sb.AppendLine("CRC Enabled\t\t\t\t = " + _configuration.CrcEnabled);
            sb.AppendLine("CRC Encoding\t\t\t = " + _configuration.CrcEncodingScheme.GetName());
            sb.AppendLine("PA Power\t\t\t\t = " + _configuration.PowerLevel.GetName());
            sb.AppendLine("Retransmit Delay\t\t = " + _configuration.AutoRetransmitDelay.GetName());
            sb.AppendLine("Retransmit Count\t\t = " + _configuration.AutoRetransmitCount);
            sb.AppendLine("Device Status\t\t\t = " + _radio.Status.GetName());
            sb.AppendLine("Transmit FIFO\t\t\t = " + _radio.TransmitPipe.FifoStatus.GetName());
            sb.AppendLine("Receive FIFO\t\t\t = " + _radio.ReceivePipes[0].FifoStatus.GetName());
            sb.AppendLine("Power Detector\t\t\t = " + _registerContainer.ReceivedPowerDetectorRegister.ReceivedPowerDetector);
            return sb.ToString();
        }
    }
}
