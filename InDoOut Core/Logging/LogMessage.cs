using System;
using System.Reflection;

namespace InDoOut_Core.Logging
{
    /// <summary>
    /// Represents a single logged message.
    /// </summary>
    public class LogMessage
    {
        /// <summary>
        /// The severity level of a log.
        /// </summary>
        public enum LogLevel
        {
            /// <summary>
            /// A simple info message. Nothing critical to program operation.
            /// </summary>
            Info,
            /// <summary>
            /// A warning message. These do not impede the program operation, 
            /// nor have they effected the program operation, but should be noted down.
            /// </summary>
            Warning,
            /// <summary>
            /// An unintended exception has occurred and shouldn't have. These are unexpected
            /// events and should not happen.
            /// </summary>
            Error
        }

        /// <summary>
        /// The log message. This contains the main part of the log that the user should see.
        /// </summary>
        public object Message { get; protected set; }

        /// <summary>
        /// The assembly that activated this message. This can be used for filtering and tracing.
        /// </summary>
        public Assembly CallingAssembly { get; private set; }

        /// <summary>
        /// The log level of this log message.
        /// </summary>
        public LogLevel Level { get; protected set; }

        /// <summary>
        /// The time that this message was logged.
        /// </summary>
        public DateTime Time { get; private set; }

        /// <summary>
        /// Creates a basic <see cref="LogMessage"/>.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="level">The level of the current log.</param>
        /// <param name="calledFrom">The assembly that activated this log.</param>
        public LogMessage(object message, LogLevel level, Assembly calledFrom) : this(message, level, calledFrom, DateTime.Now)
        {
        }

        /// <summary>
        /// Creates a basic <see cref="LogMessage"/>, with manual entry of the <paramref name="time"/>.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="level">The level of the current log.</param>
        /// <param name="calledFrom">The assembly that activated this log.</param>
        /// <param name="time">The time that this log message occurred.</param>
        public LogMessage(object message, LogLevel level, Assembly calledFrom, DateTime time)
        {
            Message = message;
            Level = level;
            Time = time;
            CallingAssembly = calledFrom;
        }

        /// <summary>
        /// Converts the log message to a string.
        /// </summary>
        /// <returns>The log message in string form.</returns>
        public override string ToString()
        {
            return $"[{Time.ToString("dd/MM/yyyy")}][{Level}] {Message}";
        }
    }
}
