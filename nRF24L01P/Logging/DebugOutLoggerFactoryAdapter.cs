using Common.Logging;
using Common.Logging.Configuration;
using Common.Logging.Simple;

namespace Windows.Devices.Radios.nRF24L01P.Logging
{
    public class DebugOutLoggerFactoryAdapter : AbstractSimpleLoggerFactoryAdapter
    {
        public DebugOutLoggerFactoryAdapter()
          : base(null)
        {
        }

        public DebugOutLoggerFactoryAdapter(NameValueCollection properties)
          : base(properties)
        {
        }

        public DebugOutLoggerFactoryAdapter(LogLevel level, bool showDateTime, bool showLogName, bool showLevel, string dateTimeFormat)
          : base(level, showDateTime, showLogName, showLevel, dateTimeFormat)
        {
        }

        protected override ILog CreateLogger(string name, LogLevel level, bool showLevel, bool showDateTime, bool showLogName, string dateTimeFormat)
        {
            return new DebugOutLogger(name, level, showLevel, showDateTime, showLogName, dateTimeFormat);
        }
    }
}
