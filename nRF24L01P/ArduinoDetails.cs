using System.Diagnostics;
using System.Linq;
using System.Text;
using Windows.Devices.Radios.nRF24L01P.Enums;
using Windows.Devices.Radios.nRF24L01P.Extensions;
using Windows.Devices.Radios.nRF24L01P.Interfaces;
using Windows.Devices.Radios.nRF24L01P.Registers;

namespace Windows.Devices.Radios.nRF24L01P
{
    /// <summary>
    /// This class is used to produce details similar to the printDetails() Arduino method 
    /// </summary>
    public class ArduinoDetails
    {
        private readonly IRadio _radio;
        private readonly IConfiguration _configuration;
        private readonly ICommandProcessor _commandProcessor;
        private readonly IRegisterContainer _registerContainer;

        public ArduinoDetails(IRadio radio, IConfiguration configuration, ICommandProcessor commandProcessor, IRegisterContainer registerContainer)
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
            string registerValue = string.Format("{0}{1}\t =", name, extraTab);
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
            string registerValue = string.Format("{0}{1}\t =", name, extraTab);
            for (int index = 0; index < quantity; index++)
                registerValue += " 0x" + _commandProcessor.ExecuteCommand(DeviceCommands.R_REGISTER, register++, new byte[1])[0].ToString("X").PadLeft(2, '0');

            return registerValue;
        }

        public void PrintDetails()
        {
            Debug.WriteLine(this);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            StatusRegister statusRegister = _registerContainer.StatusRegister;
            statusRegister.Load();
            byte status = statusRegister;
            sb.AppendFormat("STATUS\t\t\t = 0x{0} RX_DR={1} TX_DS={2} MAX_RT={3} RX_P_NO={4} TX_FULL={5}\r\n",
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

            sb.AppendLine("Data Rate\t\t = " + _configuration.DataRate.GetName());
            sb.AppendLine("Model\t\t\t = " + _configuration.RadioModel.GetName());
            sb.AppendLine("CRC Length\t\t = " + _configuration.CrcEncodingScheme.GetName());
            sb.AppendLine("PA Power\t\t = " + _configuration.PowerLevel.GetName());

            return sb.ToString();
        }
    }
}
