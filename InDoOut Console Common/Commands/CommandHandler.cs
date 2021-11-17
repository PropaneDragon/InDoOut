using InDoOut_Console_Common.ConsoleExtensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace InDoOut_Console_Common.Commands
{
    public class CommandHandler
    {
        public bool Exit { get; set; } = false;

        private List<ICommand> Commands { get; } = new List<ICommand>();

        public CommandHandler()
        {
        }

        public void AddCommand(ICommand command)
        {
            if (command != null)
            {
                Commands.Add(command);
            }
        }

        public bool AwaitCommands()
        {
            while (!Exit)
            {
                var commandLine = ReadCommand();
                if (commandLine != null)
                {
                    var command = commandLine.Trim(' ');
                    var parameters = "";
                    var splitPoint = commandLine.IndexOf(' ');

                    if (splitPoint != -1)
                    {
                        parameters = command[splitPoint..].Trim(' ');
                        command = command[..splitPoint];
                    }

                    var foundCommands = Commands.Where(foundCommand => foundCommand.Matches(command));
                    if (foundCommands.Count() == 1)
                    {
                        var foundCommand = foundCommands.First();
                        if (foundCommand != null)
                        {
                            _ = foundCommand.Trigger(parameters);
                        }
                    }
                    else if (foundCommands.Count() > 1)
                    {
                        ConsoleFormatter.DrawErrorMessageLine("Multiple commands matched \"", ConsoleFormatter.AccentTertiary, command, ConsoleFormatter.Primary, "\". Please try a different command.");
                    }
                    else
                    {
                        ConsoleFormatter.DrawErrorMessageLine("Command \"", ConsoleFormatter.AccentTertiary, command, ConsoleFormatter.Primary, "\" not found. Please try a different command.");
                    }
                }
            }

            return true;
        }

        private string ReadCommand()
        {
            //ExtendedConsole.ResetColoursAfterWrite = false;

            ConsoleFormatter.DrawInfoMessage("Command: ", ConsoleFormatter.AccentTertiary);

            var command = Console.ReadLine();

            //ExtendedConsole.ResetColoursAfterWrite = true;

            return command;
        }

        private string ReadCommandChars()
        {
            var command = "";
            var exit = false;

            ExtendedConsole.ResetColoursAfterWrite = false;

            ConsoleFormatter.DrawInfoMessage("Command: ", ConsoleFormatter.AccentTertiary);

            while (!exit)
            {
                var keyInfo = Console.ReadKey();
                var key = keyInfo.Key;
                var raw = keyInfo.KeyChar;

                if (key == ConsoleKey.Enter)
                {
                    exit = true;
                }
                else
                {
                    command += raw;
                }
            }

            ExtendedConsole.ResetColoursAfterWrite = true;

            return command;
        }
    }
}
