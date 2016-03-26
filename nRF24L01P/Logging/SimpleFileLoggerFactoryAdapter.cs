using Common.Logging;
using Common.Logging.Configuration;
using Common.Logging.Simple;

namespace Windows.Devices.Radios.nRF24L01P.Logging
{
    public class SimpleFileLoggerFactoryAdapter : AbstractSimpleLoggerFactoryAdapter
    {
        private readonly string _logFileName;
        public SimpleFileLoggerFactoryAdapter(NameValueCollection properties) : base(properties)
        {
        }

        public SimpleFileLoggerFactoryAdapter(LogLevel level, bool showDateTime, bool showLogName, bool showLevel, string dateTimeFormat, string logFileName)
            : base(level, showDateTime, showLogName, showLevel, dateTimeFormat)
        {
            _logFileName = logFileName;
        }

        protected override ILog CreateLogger(string name, LogLevel level, bool showLevel, bool showDateTime, bool showLogName,
            string dateTimeFormat)
        {
            return new SimpleFileLogger(name, level, showLevel, showDateTime, showLogName, dateTimeFormat, _logFileName);
        }
    }
}
