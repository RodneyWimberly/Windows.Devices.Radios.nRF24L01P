using Common.Logging;
using System;
using System.Diagnostics;
using System.Reflection;
using Windows.UI.Xaml;

namespace Windows.Devices.Radios.nRF24L01P.Common.Extensions
{
    public static class ExceptionExtensions
    {
        private static ILog _currentLogger;

        public static ILog CurrentLogger
        {
            get { return _currentLogger ?? LogManager.GetCurrentClassLogger(); }
            set { _currentLogger = value; }
        }

        public static void SendToLog(this Exception exception)
        {
            exception.SendToLog(CurrentLogger);
        }

        public static void SendToLog(this Exception exception, string message)
        {
            exception.SendToLog(CurrentLogger, message);
        }

        public static void SendToLog(this Exception exception, ILog log)
        {
            exception.SendToLog(log, string.Format("Method {0} has thrown the following exception:", exception.GetMethodName()));
        }

        public static void SendToLog(this Exception exception, ILog log, string message)
        {
            log.Error(message, exception);
        }
    
        public static string GetMethodName(this Exception exception)
        {
            string[] stackList = exception.StackTrace.Split('\n');
            return stackList[0].Replace("at ", string.Empty).Replace("\r", string.Empty).Trim();
        }

        public static void LogUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception exception = e.Exception;
            exception.SendToLog(CurrentLogger, string.Format("An un-handled exception occurred in method {0}", exception.GetMethodName()));
        }

        public static void ExecuteFunction(ILog logger, Action action, bool reThrow = true)
        {
            string methodName = action.GetMethodInfo().Name;
            Stopwatch stopwatch = Stopwatch.StartNew();
            if(logger.IsTraceEnabled) logger.TraceFormat("Method {0} started at {1}", methodName, DateTime.Now);
            try { action.Invoke(); }
            catch (Exception ex)
            {
                ex.SendToLog(logger, string.Format("Method {0} has thrown the following exception:", methodName));
                if (reThrow) throw;
            }
            finally
            {
                if (logger.IsTraceEnabled) logger.TraceFormat("Method {0} completed in {1} seconds", methodName, stopwatch.Elapsed);
            }
        }

        public static T ExecuteFunction<T>(ILog logger, Func<T> function, bool reThrow = true)
        {
            T returnValue = default(T);
            string methodName = function.GetMethodInfo().Name;
            Stopwatch stopwatch = Stopwatch.StartNew();
            if (logger.IsTraceEnabled) logger.TraceFormat("Method {0} started at {1}", methodName, DateTime.Now);
            try { returnValue = function.Invoke(); }
            catch (Exception ex)
            {
                ex.SendToLog(logger, string.Format("Method {0} has thrown the following exception:", methodName));
                if(reThrow) throw;
            }
            finally
            {
                if (logger.IsTraceEnabled) logger.TraceFormat("Method {0} completed in {1} seconds", methodName, stopwatch.Elapsed);
            }

            return returnValue;
        }
    }
}
