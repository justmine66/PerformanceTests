using System;

namespace PerformanceTests.Infrastructure.Logging
{
    /// <summary>
    /// 控制台日志器
    /// </summary>
    public class ConsoleLogger : ILogger
    {
        public bool IsDebugEnabled => true;

        public void Debug(object message)
        {
            ConsoleOutputUtils.DebugFormat(message + string.Empty);
        }

        public void Debug(object message, Exception exception)
        {
            ConsoleOutputUtils.DebugFormat("{0}, exception: {1}", message, exception.ToString());
        }

        public void DebugFormat(string format, params object[] args)
        {
            ConsoleOutputUtils.DebugFormat(format, args);
        }

        public void Info(object message)
        {
            ConsoleOutputUtils.InfoFormat(message + string.Empty);
        }

        public void Info(object message, Exception exception)
        {
            ConsoleOutputUtils.InfoFormat("{0}, exception: {1}", message, exception.ToString());
        }

        public void InfoFormat(string format, params object[] args)
        {
            ConsoleOutputUtils.InfoFormat(format, args);
        }

        public void Warn(object message)
        {
            ConsoleOutputUtils.WarnFormat(message + string.Empty);
        }

        public void Warn(object message, Exception exception)
        {
            ConsoleOutputUtils.WarnFormat("{0}, exception: {1}", message, exception.ToString());
        }

        public void WarnFormat(string format, params object[] args)
        {
            ConsoleOutputUtils.WarnFormat(format, args);
        }

        public void Error(object message)
        {
            ConsoleOutputUtils.ErrorFormat(message + string.Empty);
        }

        public void Error(object message, Exception exception)
        {
            ConsoleOutputUtils.ErrorFormat("{0}, exception: {1}", message, exception.ToString());
        }

        public void ErrorFormat(string format, params object[] args)
        {
            ConsoleOutputUtils.ErrorFormat(format, args);
        }

        public void Fatal(object message)
        {
            ConsoleOutputUtils.FatalFormat(message + string.Empty);
        }

        public void Fatal(object message, Exception exception)
        {
            ConsoleOutputUtils.FatalFormat("{0}, exception: {1}", message, exception.ToString());
        }

        public void FatalFormat(string format, params object[] args)
        {
            ConsoleOutputUtils.FatalFormat(format, args);
        }
    }
}
