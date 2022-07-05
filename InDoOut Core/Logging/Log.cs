using InDoOut_Core.Instancing;
using System.Collections.Generic;
using System.Reflection;

namespace InDoOut_Core.Logging
{
    /// <summary>
    /// Handles logs between all elements of IDO. Thread safe and can be called from anywhere.
    /// </summary>
    public class Log : Singleton<Log>, ILog
    {
        private readonly object _logLock = new();
        private readonly List<LogMessage> _logs = new();

        /// <summary>
        /// Gets or sets whether logging is active or not.
        /// </summary>
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// The maximum number of log messages allowed to be kept at once.
        /// </summary>
        public int MaxLogMessages { get; set; } = 10000;

        /// <summary>
        /// The name of the log
        /// </summary>
        public string Name { get; set; } = "Core";

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
        /// Creates a core logger. 
        /// </summary>
        protected Log() : this("Core")
        {

        }

        /// <summary>
        /// Creates a log with a name
        /// </summary>
        /// <param name="name">The name of the log to be created</param>
        public Log(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Logs an error with the given message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public void Error(params object[] message)
        {
            if (Enabled)
            {
                var calledFrom = Assembly.GetCallingAssembly();

                CreateEntry(new LogMessage(LogMessage.LogLevel.Error, calledFrom, message));
            }
        }

        /// <summary>
        /// Logs a warning with the given message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public void Warning(params object[] message)
        {
            if (Enabled)
            {
                var calledFrom = Assembly.GetCallingAssembly();

                CreateEntry(new LogMessage(LogMessage.LogLevel.Warning, calledFrom, message));
            }
        }

        /// <summary>
        /// Logs info with the given message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public void Info(params object[] message)
        {
            if (Enabled)
            {
                var calledFrom = Assembly.GetCallingAssembly();

                CreateEntry(new LogMessage(LogMessage.LogLevel.Info, calledFrom, message));
            }
        }

        /// <summary>
        /// Logs info with the given message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public void Header(params object[] message)
        {
            if (Enabled)
            {
                var calledFrom = Assembly.GetCallingAssembly();

                CreateEntry(new LogMessage(LogMessage.LogLevel.Header, calledFrom, message));
            }
        }

        private void CreateEntry(LogMessage message)
        {
            if (Enabled && message != null)
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
