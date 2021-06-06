using InDoOut_Console_Common.ConsoleExtensions;
using InDoOut_Core.Functions;
using InDoOut_Executable_Core.Networking.Commands;
using InDoOut_Executable_Core.Programs;
using InDoOut_Networking.Server;
using InDoOut_Networking.Server.Commands;
using InDoOut_Plugins.Loaders;
using System.Collections.Generic;
using System.Linq;

namespace InDoOut_Server.Loading
{
    public class ConsoleCommandLoader
    {
        private readonly IProgramHolder _programHolder = null;
        private readonly IServer _server = null;

        public ConsoleCommandLoader(IServer server, IProgramHolder programHolder)
        {
            _server = server;
            _programHolder = programHolder;
        }

        public bool Load()
        {
            ExtendedConsole.WriteLine();
            ConsoleFormatter.DrawInfoMessageLine("Registering server commands...");

            var results = new List<bool>
            {
                AddCommandListener(new RequestProgramsServerCommand(_server, _programHolder)),
                AddCommandListener(new UploadProgramServerCommand(_server, _programHolder, LoadedPlugins.Instance, new FunctionBuilder())),
                AddCommandListener(new DownloadProgramServerCommand(_server, _programHolder, LoadedPlugins.Instance, new FunctionBuilder())),
                AddCommandListener(new GetProgramStatusServerCommand(_server, _programHolder))
            };

            var totalCommands = results.Count;
            var successfulCommands = results.Count(result => result);
            var unsuccessfulCommands = totalCommands - successfulCommands;

            ConsoleFormatter.DrawInfoMessageLine("Registered ", ConsoleFormatter.AccentTertiary, results.Count(result => result), ConsoleFormatter.Primary, " of ", ConsoleFormatter.AccentTertiary, results.Count, ConsoleFormatter.Primary, " commands.");

            if (unsuccessfulCommands > 0)
            {
                ConsoleFormatter.DrawErrorMessageLine(ConsoleFormatter.AccentTertiary, unsuccessfulCommands, ConsoleFormatter.Negative, $" command{(unsuccessfulCommands != 1 ? "s" : "")} failed to register. Some functionality may not work!");
            }

            return true;
        }

        private bool AddCommandListener(ICommandListener listener)
        {
            if (_server != null)
            {
                if (listener != null)
                {
                    ConsoleFormatter.DrawInfoMessage(ConsoleFormatter.AccentTertiary, "  > ", ConsoleFormatter.Primary, "Registering ", ConsoleFormatter.AccentTertiary, listener.CommandName, ConsoleFormatter.Primary, " command... ");

                    var added = _server.AddCommandListener(listener);
                    if (added)
                    {
                        ExtendedConsole.WriteLine(ConsoleFormatter.Positive, "Success.");
                    }
                    else
                    {
                        ExtendedConsole.WriteLine(ConsoleFormatter.Negative, "Failed!");
                    }

                    return added;
                }
                else
                {
                    ConsoleFormatter.DrawInfoMessageLine(ConsoleFormatter.Negative, "Attempted to add an invalid command listener to the server!");
                }
            }
            else
            {
                ConsoleFormatter.DrawInfoMessageLine(ConsoleFormatter.Negative, "Attempted to add a listener to a non-existant server!");
            }

            return false;
        }
    }
}
