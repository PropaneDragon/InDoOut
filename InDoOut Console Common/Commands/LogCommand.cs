using InDoOut_Console_Common.ConsoleExtensions;
using InDoOut_Core.Logging;
using System.Collections.Generic;
using System.Linq;

namespace InDoOut_Console_Common.Commands
{
    public class LogCommand : SimpleCommand
    {
        public List<ILog> Logs { get; set; }

        public LogCommand(params ILog[] logs) : base("log")
        {
            Logs = logs.ToList();
        }

        public override bool Trigger(string[] parameters)
        {
            var limit = 20;

            if (parameters.Length > 0 && int.TryParse(parameters[0], out var newLimit))
            {
                limit = newLimit;
            }

            foreach (var log in Logs)
            {
                if (log != null)
                {
                    DisplayRecentLogs(log, limit);
                }
            }

            return true;
        }

        public void DisplayLogTitle(ILog log)
        {
            if (log != null)
            {
                ConsoleFormatter.DrawSubtitle(log.Name, ConsoleFormatter.AccentTertiary, $" ({log.Logs.Count})");
            }
        }

        public void DisplayRecentLogs(ILog log, int limit = 20)
        {
            if (log != null)
            {
                var recentMessages = log.Logs.TakeLast(limit);

                ConsoleFormatter.DrawSubtitle(log.Name, ConsoleFormatter.AccentTertiary, $" (showing {recentMessages.Count()} of {log.Logs.Count})");

                foreach (var message in recentMessages)
                {
                    DisplayLog(message);
                }
            }
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
                        ExtendedConsole.Write(ConsoleFormatter.AccentPrimary, "[ title ] ");
                        break;
                    case LogMessage.LogLevel.Info:
                        ConsoleFormatter.DrawInfoMessage();
                        break;
                    case LogMessage.LogLevel.Warning:
                        ConsoleFormatter.DrawWarningMessage();
                        break;
                }

                ExtendedConsole.Write(ConsoleFormatter.AccentPrimary, log.Time.ToString("[HH:mm:ss.ff] "), ConsoleFormatter.Primary);
                ExtendedConsole.WriteLine(log.Message);
            }
        }
    }
}
