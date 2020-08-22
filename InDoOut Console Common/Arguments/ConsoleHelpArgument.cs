using InDoOut_Console_Common.ConsoleExtensions;
using InDoOut_Executable_Core.Arguments;
using System;
using System.Linq;

namespace InDoOut_Console_Common.Arguments
{
    public class ConsoleHelpArgument : AbstractHelpArgument
    {
        public ConsoleHelpArgument(bool showHelpWhenTriggered = true) : base(showHelpWhenTriggered)
        {
        }

        public override void ShowHelp(IArgumentHandler handler)
        {
            if (handler != null)
            {
                ConsoleFormatter.DrawInfoMessage("Commands are accepted with these prefixes: ", ConsoleFormatter.YellowPastel, string.Join(' ', handler.ArgumentKeyPrefixes));
                ConsoleFormatter.DrawInfoMessage("Add values to commands (if supported) with either a space, or the following delimiters: ", ConsoleFormatter.YellowPastel, string.Join(' ', handler.KeyValueSeparators));
                ConsoleFormatter.DrawInfoMessage("For example, these commands would be valid:");

                PrintAllAvailableCommands(handler);
                Console.WriteLine();

                ConsoleFormatter.DrawInfoMessage("Available commands:");

                foreach (var argument in handler.Arguments)
                {
                    PrintArgument(argument);
                }
            }
        }

        private void PrintAllAvailableCommands(IArgumentHandler handler)
        {
            _ = handler.ArgumentKeyPrefixes;

            var exampleCommand = "command";
            var exampleValue = "value";
            var availablePrefixes = handler.ArgumentKeyPrefixes;
            var availableKeyValueSeparators = handler.KeyValueSeparators.ToList();

            availableKeyValueSeparators.Add(' ');

            foreach (var prefix in availablePrefixes)
            {
                foreach (var keyValueSeparator in availableKeyValueSeparators)
                {
                    ExtendedConsole.WriteLine("     ", ConsoleFormatter.YellowPastel, prefix, ConsoleFormatter.RedPastel, exampleCommand, ConsoleFormatter.YellowPastel, keyValueSeparator, ConsoleFormatter.PurplePastel, exampleValue);
                }
            }
        }

        private void PrintArgument(IArgument argument)
        {
            if (!(argument?.Hidden ?? true))
            {
                ExtendedConsole.Write("     ", ConsoleFormatter.YellowPastel, "-", ConsoleFormatter.RedPastel, argument.Key);

                if (argument.AllowsValue)
                {
                    ExtendedConsole.Write(ConsoleFormatter.YellowPastel, ":", ConsoleFormatter.PurplePastel, "[value]");
                }

                ExtendedConsole.WriteLine(ConsoleFormatter.BluePastel, $" - {argument.Description}");
            }
        }
    }
}
