using InDoOut_Console.Display;
using InDoOut_Executable_Core.Arguments;
using System;
using System.Linq;

namespace InDoOut_Console.Arguments
{
    public class ConsoleHelpArgument : AbstractHelpArgument
    {
        public bool HelpShown { get; private set; } = false;

        protected override void ShowHelp(IArgumentHandler handler)
        {
            if (handler != null)
            {
                ColourConsole.WriteInfoLine($"Commands are accepted with these prefixes: {string.Join(' ', handler.ArgumentKeyPrefixes)}");
                ColourConsole.WriteInfoLine($"Add values to commands (if supported) with either a space, or the following delimiters: {string.Join(' ', handler.KeyValueSeparators)}");
                ColourConsole.WriteInfoLine("For example, these commands would be valid:");
                PrintAllAvailableCommands(handler);
                Console.WriteLine();
                ColourConsole.WriteInfoLine("Available commands:");

                foreach (var argument in handler.Arguments)
                {
                    ColourConsole.WriteLine(handler.FormatDescription(argument));
                }
            }

            HelpShown = true;
        }

        private void PrintAllAvailableCommands(IArgumentHandler handler)
        {
            var exampleCommand = "command";
            var exampleValue = "value";
            var availablePrefixes = handler.ArgumentKeyPrefixes;
            var availableKeyValueSeparators = handler.KeyValueSeparators.ToList();

            availableKeyValueSeparators.Add(' ');

            foreach (var prefix in availablePrefixes)
            {
                foreach (var keyValueSeparator in availableKeyValueSeparators)
                {
                    ColourConsole.WriteLine(new ColourBlock(prefix, System.ConsoleColor.Yellow), new ColourBlock(exampleCommand, System.ConsoleColor.Green), new ColourBlock(keyValueSeparator, System.ConsoleColor.Yellow), new ColourBlock(exampleValue, System.ConsoleColor.Red));
                }
            }
        }
    }
}
