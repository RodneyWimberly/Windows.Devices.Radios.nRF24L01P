using Common.Logging;
using Common.Logging.Simple;
using System;
using System.Text;

namespace Windows.Devices.Radios.nRF24L01P.Logging
{
    public class DebugOutLogger : AbstractSimpleLogger
    {
        public DebugOutLogger(string logName, LogLevel logLevel, bool showLevel, bool showDateTime, bool showLogName, string dateTimeFormat)
          : base(logName, logLevel, showLevel, showDateTime, showLogName, dateTimeFormat)
        {
        }

        protected override void WriteInternal(LogLevel level, object message, Exception e)
        {
            StringBuilder output = new StringBuilder();
            this.FormatOutput(output, level, message, e);
            global::System.Diagnostics.Debug.WriteLine(output);
        }
    }
}
