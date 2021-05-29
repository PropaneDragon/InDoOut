using InDoOut_Core.Logging;
using System.Collections.Generic;

namespace InDoOut_Console_Common.Messaging
{
    public class ConsoleLog : ILog
    {
        private readonly Log _internalLog = new Log();

        public bool Enabled { get => _internalLog.Enabled; set => _internalLog.Enabled = value; }

        public List<LogMessage> Logs => _internalLog.Logs;

        public int MaxLogMessages { get => _internalLog.MaxLogMessages; set => _internalLog.MaxLogMessages = value; }

        public void Error(params object[] message) => _internalLog.Error(message);
        public void Header(params object[] message) => _internalLog.Header(message);
        public void Info(params object[] message) => _internalLog.Info(message);
        public void Warning(params object[] message) => _internalLog.Warning(message);
    }
}
