using InDoOut_Console_Common.ConsoleExtensions;
using InDoOut_Core.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace InDoOut_Server.ServerLogging
{
    public class ConsoleLogger
    {
        private CancellationTokenSource _cancellationToken = null;
        private Task _listenerTask = null;

        public ILog Log { get; set; } = null;

        public ConsoleLogger(ILog log)
        {
            Log = log;
        }

        public bool StartListening()
        {
            _ = StopListening();

            _cancellationToken = new CancellationTokenSource();

            _listenerTask = Task.Run(() =>
            {
                var lastLogTime = DateTime.Now;

                while (!_cancellationToken.IsCancellationRequested && Log != null)
                {
                    var newLogs = Log.Logs.Where(log => log.Time > lastLogTime);

                    foreach (var newLog in newLogs)
                    {
                        DisplayLog(newLog);
                    }

                    lastLogTime = DateTime.Now;

                    Thread.Sleep(TimeSpan.FromMilliseconds(500));
                }
            });

            return true;
        }

        public bool StopListening()
        {
            if (_cancellationToken != null)
            {
                _cancellationToken.Cancel();

                return true;
            }

            return false;
        }

        private void DisplayLog(LogMessage log)
        {
            if (log != null)
            {
                switch (log.Level)
                {
                    case LogMessage.LogLevel.Error:
                        ConsoleFormatter.DrawErrorMessage();
                        break;
                    case LogMessage.LogLevel.Header:
                        ConsoleFormatter.DrawSubtitle(log.Message);
                        break;
                    case LogMessage.LogLevel.Info:
                        ConsoleFormatter.DrawInfoMessage();
                        break;
                    case LogMessage.LogLevel.Warning:
                        ConsoleFormatter.DrawWarningMessage();
                        break;
                }

                if (log.Level != LogMessage.LogLevel.Header)
                {
                    ExtendedConsole.Write(ConsoleFormatter.AccentPrimary, log.Time.ToString("[dd/MM/yy HH:mm:ss.ff] "), ConsoleFormatter.Primary);
                    ExtendedConsole.WriteLine(log.Message);
                }
            }
        }
    }
}
