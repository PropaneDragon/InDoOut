using InDoOut_Console_Common.ConsoleExtensions;
using System;

namespace InDoOut_Console_Common.Loading
{
    public abstract class ConsoleLoader : IConsoleLoader
    {
        public abstract string Name { get; }

        public bool Load()
        {
            ExtendedConsole.WriteLine();
            ConsoleFormatter.DrawInfoMessageLine($"Started {Name ?? "Unknown process"}...");

            var result = BeginLoad();

            ConsoleFormatter.DrawInfoMessageLine($"Completed {Name ?? "Unknown process"}.");
            ExtendedConsole.WriteLine();

            return result;
        }

        protected void WriteMessageLine(params object[] message)
        {
            WriteStart();
            ExtendedConsole.Write(message);
            WriteEnd();
        }

        protected bool WriteMessageLine(bool outcome, params object[] message)
        {
            WriteStart();
            ExtendedConsole.Write(message);
            return WriteEnd(outcome);
        }

        protected bool WriteMessageLine(Func<bool> processFunction, params object[] message)
        {
            WriteStart();
            ExtendedConsole.Write(message);
            return WriteEnd(processFunction);
        }

        protected void WriteHighlight(string message) => ExtendedConsole.Write(ConsoleFormatter.AccentTertiary, message, ConsoleFormatter.Primary);

        protected void WriteStart() => ConsoleFormatter.DrawInfoMessage(ConsoleFormatter.AccentTertiary, "  > ", ConsoleFormatter.Primary);

        protected void WriteEnd() => ExtendedConsole.WriteLine();

        protected bool WriteEnd(bool success)
        {
            var colour = success ? ConsoleFormatter.Positive : ConsoleFormatter.Negative;
            var message = success ? "Success." : "Failed.";

            ExtendedConsole.WriteLine(colour, message);

            return success;
        }

        protected bool WriteEnd(Func<bool> processFunction) => WriteEnd(processFunction?.Invoke() ?? false);

        protected abstract bool BeginLoad();
    }
}
