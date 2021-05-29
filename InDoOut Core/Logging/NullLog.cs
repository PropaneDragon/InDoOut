using System.Collections.Generic;

namespace InDoOut_Core.Logging
{
    /// <summary>
    /// A null log that does nothing.
    /// </summary>
    public class NullLog : ILog
    {
        /// <summary>
        /// See <see cref="ILog.Enabled"/>
        /// </summary>
        public bool Enabled { get => true; set { } }

        /// <summary>
        /// See <see cref="ILog.Logs"/>
        /// </summary>
        public List<LogMessage> Logs => new List<LogMessage>();

        /// <summary>
        /// See <see cref="ILog.MaxLogMessages"/>
        /// </summary>
        public int MaxLogMessages { get => 0; set { } }

        /// <summary>
        /// See <see cref="ILog.Error(object[])"/>
        /// </summary>
        /// <param name="message"></param>
        public void Error(params object[] message) { }

        /// <summary>
        /// See <see cref="ILog.Header(object[])"/>
        /// </summary>
        /// <param name="message"></param>
        public void Header(params object[] message) { }

        /// <summary>
        /// See <see cref="ILog.Info(object[])"/>
        /// </summary>
        /// <param name="message"></param>
        public void Info(params object[] message) { }

        /// <summary>
        /// See <see cref="ILog.Warning(object[])"/>
        /// </summary>
        /// <param name="message"></param>
        public void Warning(params object[] message) { }
    }
}
