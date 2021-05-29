using System.Collections.Generic;

namespace InDoOut_Core.Logging
{
    /// <summary>
    /// A type of logger that can receive log messages and save/print them somewhere.
    /// </summary>
    public interface ILog
    {
        /// <summary>
        /// If the logger is enabled.
        /// </summary>
        bool Enabled { get; set; }

        /// <summary>
        /// A list of log messages queued for reading.
        /// </summary>
        List<LogMessage> Logs { get; }

        /// <summary>
        /// The maximum number of log messages before old logs are unqueued.
        /// </summary>
        int MaxLogMessages { get; set; }

        /// <summary>
        /// Logs an error message.
        /// </summary>
        /// <param name="message">A list of objects to log.</param>
        void Error(params object[] message);

        /// <summary>
        /// Logs a message as a header.
        /// </summary>
        /// <param name="message">A list of objects to log.</param>
        void Header(params object[] message);

        /// <summary>
        /// Logs an info message.
        /// </summary>
        /// <param name="message">A list of objects to log.</param>
        void Info(params object[] message);

        /// <summary>
        /// Logs a warning message.
        /// </summary>
        /// <param name="message">A list of objects to log.</param>
        void Warning(params object[] message);
    }
}