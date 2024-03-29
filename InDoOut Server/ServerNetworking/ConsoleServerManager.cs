﻿using InDoOut_Console_Common.ConsoleExtensions;
using InDoOut_Console_Common.Loading;
using InDoOut_Core.Logging;
using InDoOut_Executable_Core.Programs;
using InDoOut_Networking.Server;
using InDoOut_Networking.Server.Events;
using InDoOut_Server.Loading;
using System.Net;

namespace InDoOut_Server.ServerNetworking
{
    public class ConsoleServerManager
    {
        private readonly ProgramHolder _programHolder = new();
        private readonly Server _server = null;
        private readonly ILog _serverLog = null;
        private readonly ILog _eventLog = null;

        public ConsoleServerManager(ILog serverLog, ILog eventLog, int port = 0)
        {
            ConsoleFormatter.DrawInfoMessageLine("Starting up the manager...");

            _serverLog = serverLog;
            _eventLog = eventLog;

            _server = new Server(_serverLog, port);

            var commandLoader = new ConsoleCommandLoader(_server, _programHolder);
            var pluginLoader = new ConsolePluginLoader();

            _ = commandLoader.Load();
            _ = pluginLoader.Load();

            ExtendedConsole.WriteLine();

            HookServerEvents(_server);

            ConsoleFormatter.DrawInfoMessageLine(ConsoleFormatter.Positive, "Manager started");
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
        private void Server_OnClientDisconnected(object sender, ClientConnectionEventArgs e) => _eventLog?.Info("Client at IP ", ConsoleFormatter.AccentTertiary, (e?.Client?.Client?.RemoteEndPoint as IPEndPoint)?.Address?.ToString() ?? "unknown", ConsoleFormatter.Negative, " disconnected.");
        private void Server_OnClientConnected(object sender, ClientConnectionEventArgs e) => _eventLog?.Info("Client at IP ", ConsoleFormatter.AccentTertiary, (e?.Client?.Client?.RemoteEndPoint as IPEndPoint)?.Address?.ToString() ?? "unknown", ConsoleFormatter.Positive, " connected.");
    }
}
