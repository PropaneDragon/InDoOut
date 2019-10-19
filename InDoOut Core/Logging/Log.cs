using InDoOut_Core.Instancing;
using System.Collections.Generic;
using System.Reflection;

namespace InDoOut_Core.Logging
{
    /// <summary>
    /// Handles logs between all elements of IDO. Thread safe and can be called from anywhere.
    /// </summary>
    public class Log : Singleton<Log>
    {
        private readonly object _logLock = new object();
        private readonly List<LogMessage> _logs = new List<LogMessage>();

        /// <summary>
        /// The maximum number of log messages allowed to be kept at once.
        /// </summary>
        public int MaxLogMessages { get; set; } = 10000;

        /// <summary>
        /// All current logs that have been sent so far, up to <see cref="MaxLogMessages"/>.
        /// </summary>
        public List<LogMessage> Logs
        {
            get
            {
                lock (_logLock)
                {
                    return new List<LogMessage>(_logs);
                }
            }
        }

        /// <summary>
        /// Logs an error with the given message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public void Error(params object[] message)
        {
            var calledFrom = Assembly.GetCallingAssembly();

            CreateEntry(new LogMessage(LogMessage.LogLevel.Error, calledFrom, message));
        }

        /// <summary>
        /// Logs a warning with the given message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public void Warning(params object[] message)
        {
            var calledFrom = Assembly.GetCallingAssembly();

            CreateEntry(new LogMessage(LogMessage.LogLevel.Warning, calledFrom, message));
        }

        /// <summary>
        /// Logs info with the given message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public void Info(params object[] message)
        {
            var calledFrom = Assembly.GetCallingAssembly();

            CreateEntry(new LogMessage(LogMessage.LogLevel.Info, calledFrom, message));
        }

        /// <summary>
        /// Logs info with the given message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public void Header(params object[] message)
        {
            var calledFrom = Assembly.GetCallingAssembly();

            CreateEntry(new LogMessage(LogMessage.LogLevel.Header, calledFrom, message));
        }

        private void CreateEntry(LogMessage message)
        {
            if (message != null)
            {
                lock (_logLock)
                {
                    _logs.Add(message);

                    if (_logs.Count > MaxLogMessages)
                    {
                        var overBy = _logs.Count - MaxLogMessages;
                        _logs.RemoveRange(0, overBy);
                    }
                }
            }
        }
    }
}
