using InDoOut_Console_Common.ConsoleExtensions;
using InDoOut_Console_Common.Messaging;
using InDoOut_Core.Functions;
using InDoOut_Executable_Core.Networking.Commands;
using InDoOut_Executable_Core.Programs;
using InDoOut_Networking.Server;
using InDoOut_Networking.Server.Commands;
using InDoOut_Networking.Server.Events;
using InDoOut_Plugins.Loaders;
using InDoOut_Server.ServerLogging;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace InDoOut_Server.ServerNetworking
{
    public class ConsoleServerManager
    {
        private readonly ProgramHolder _programHolder = new ProgramHolder();
        private readonly Server _server = null;
        private readonly ConsoleLog _log = new ConsoleLog();
        private readonly ConsoleLogger _logger = null;

        public ConsoleServerManager(int port = 0)
        {
            ConsoleFormatter.DrawInfoMessageLine("Starting up the manager...");

            _server = new Server(_log, port);
            _logger = new ConsoleLogger(_log);

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

            ConsoleFormatter.DrawInfoMessage("Starting console logger... ");

            if (_logger.StartListening())
            {
                ExtendedConsole.WriteLine(ConsoleFormatter.Positive, "Success.");
            }
            else
            {
                ExtendedConsole.WriteLine(ConsoleFormatter.Negative, "Failed!");
            }

            ExtendedConsole.WriteLine();

            HookServerEvents(_server);

            ConsoleFormatter.DrawInfoMessageLine(ConsoleFormatter.Positive, "Manager started");

#if DEBUG

            var program = _programHolder.NewProgram();
            program.SetName("Test program 1");

            program = _programHolder.NewProgram();
            program.SetName("This is another program that is also a test!");

#endif
        }

        public bool Start()
        {
            ConsoleFormatter.DrawInfoMessageLine("Manager starting the server...");

            if (_server != null)
            {
                var started = _server.Start().Result;

                if (started)
                {
                    ConsoleFormatter.DrawInfoMessageLine(ConsoleFormatter.Positive, "Manager started server.");
                }
                else
                {
                    ConsoleFormatter.DrawErrorMessageLine(ConsoleFormatter.Negative, "Manager couldn't start the server!");
                }

                return started;
            }

            return false;
        }

        public bool Stop()
        {
            ConsoleFormatter.DrawInfoMessageLine("Manager stopping the server...");

            if (_server != null)
            {
                var stopped = _server.Stop().Result;

                if (stopped)
                {
                    ConsoleFormatter.DrawInfoMessageLine(ConsoleFormatter.Positive, "Manager stopped server.");
                }
                else
                {
                    ConsoleFormatter.DrawErrorMessageLine(ConsoleFormatter.Negative, "Manager couldn't stop the server!");
                }

                return stopped;
            }

            return false;
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

        private void HookServerEvents(IServer server)
        {
            if (server != null)
            {
                server.OnClientConnected += Server_OnClientConnected;
                server.OnClientDisconnected += Server_OnClientDisconnected;
                server.OnClientMessageReceived += Server_OnClientMessageReceived;
                server.OnClientMessageSent += Server_OnClientMessageSent;
                server.OnServerStarted += Server_OnServerStarted;
                server.OnServerStopped += Server_OnServerStopped;
            }
        }

        private void UnhookServerEvents(IServer server)
        {
            if (server != null)
            {
                server.OnClientConnected -= Server_OnClientConnected;
                server.OnClientDisconnected -= Server_OnClientDisconnected;
                server.OnClientMessageReceived -= Server_OnClientMessageReceived;
                server.OnClientMessageSent -= Server_OnClientMessageSent;
                server.OnServerStarted -= Server_OnServerStarted;
                server.OnServerStopped -= Server_OnServerStopped;
            }
        }

        private void Server_OnServerStopped(object sender, ServerConnectionEventArgs e) => ConsoleFormatter.DrawInfoMessageLine(ConsoleFormatter.Negative, "Server stopped.");
        private void Server_OnServerStarted(object sender, ServerConnectionEventArgs e) => ConsoleFormatter.DrawInfoMessageLine(ConsoleFormatter.Positive, "Server started.", ConsoleFormatter.Primary, " IP: ", ConsoleFormatter.AccentTertiary, (sender as IServer)?.IPAddress?.ToString() ?? "unknown", ConsoleFormatter.Primary, ", Port: ", ConsoleFormatter.AccentTertiary, (sender as IServer)?.Port.ToString() ?? "Unknown");
        private void Server_OnClientMessageSent(object sender, ClientMessageEventArgs e) { }
        private void Server_OnClientMessageReceived(object sender, ClientMessageEventArgs e) { }
        private void Server_OnClientDisconnected(object sender, ClientConnectionEventArgs e) => ConsoleFormatter.DrawInfoMessageLine("Client at IP ", ConsoleFormatter.AccentTertiary, (e?.Client?.Client?.RemoteEndPoint as IPEndPoint)?.Address?.ToString() ?? "unknown", ConsoleFormatter.Negative, " disconnected.");
        private void Server_OnClientConnected(object sender, ClientConnectionEventArgs e) => ConsoleFormatter.DrawInfoMessageLine("Client at IP ", ConsoleFormatter.AccentTertiary, (e?.Client?.Client?.RemoteEndPoint as IPEndPoint)?.Address?.ToString() ?? "unknown", ConsoleFormatter.Positive, " connected.");
    }
}
