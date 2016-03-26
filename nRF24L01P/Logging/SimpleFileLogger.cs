using Common.Logging;
using Common.Logging.Simple;
using System;
using System.IO;
using System.Text;

namespace Windows.Devices.Radios.nRF24L01P.Logging
{
    public class SimpleFileLogger : AbstractSimpleLogger
    {
        private readonly object _syncRoot;

        public string LogFileName { get; set; }

        public SimpleFileLogger(string logName, LogLevel logLevel, bool showLevel, bool showDateTime,
            bool showLogName, string dateTimeFormat, string logFileName)
            : base(logName, logLevel, showLevel, showDateTime, showLogName, dateTimeFormat)
        {
            _syncRoot = new object();
            LogFileName = logFileName;
        }

        protected override void WriteInternal(LogLevel level, object message, Exception exception)
        {
            if (string.IsNullOrWhiteSpace(LogFileName))
                throw new ConfigurationException("LogFileName is empty and therefore the logger can't write any information");

            StringBuilder logEvent = new StringBuilder();
            FormatOutput(logEvent, level, message, exception);

            lock (_syncRoot)
            {
                string directory = Path.GetDirectoryName(LogFileName);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
                File.WriteAllText(LogFileName, logEvent.ToString());
            }
        }

        public override bool IsTraceEnabled { get; }
        public override bool IsDebugEnabled { get; }
        public override bool IsErrorEnabled { get; }
        public override bool IsFatalEnabled { get; }
        public override bool IsInfoEnabled { get; }
        public override bool IsWarnEnabled { get; }
    }
}
