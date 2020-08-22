using InDoOut_Console_Common.ConsoleExtensions;
using InDoOut_Executable_Core.Networking;
using InDoOut_Executable_Core.Networking.ServerEventArgs;
using InDoOut_Executable_Core.Programs;
using System;
using System.Net;

namespace InDoOut_Server.ServerNetworking
{
    public class ConsoleServerManager
    {
        private readonly ProgramHolder _programHolder = new ProgramHolder();
        private readonly ProgramSyncServer _server = null;

        public ConsoleServerManager(int port = 0)
        {
            _server = new ProgramSyncServer(_programHolder, port);

            HookServerEvents(_server);
        }

        public void Start()
        {
            if (_server != null)
            {
                var started = _server.Start().Result;

                if (started)
                {
                    ConsoleFormatter.DrawInfoMessageLine(ConsoleFormatter.GreenPastel, "Started.");
                }
                else
                {
                    ConsoleFormatter.DrawErrorMessageLine(ConsoleFormatter.GreenPastel, "Failed to start!");
                } 
            } 
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

        private void Server_OnServerStopped(object sender, ServerConnectionEventArgs e) => ConsoleFormatter.DrawInfoMessageLine("Server stopped.");
        private void Server_OnServerStarted(object sender, ServerConnectionEventArgs e) => ConsoleFormatter.DrawInfoMessageLine("Server started. IP: ", ConsoleFormatter.PurplePastel, (sender as IServer)?.IPAddress?.ToString() ?? "unknown", ConsoleFormatter.Primary, ", Port: ", ConsoleFormatter.PurplePastel, (sender as IServer)?.Port.ToString() ?? "Unknown");
        private void Server_OnClientMessageSent(object sender, ClientMessageEventArgs e) { }
        private void Server_OnClientMessageReceived(object sender, ClientMessageEventArgs e) { }
        private void Server_OnClientDisconnected(object sender, ClientConnectionEventArgs e) => ConsoleFormatter.DrawInfoMessageLine("Client at IP ", ConsoleFormatter.PurplePastel, (e?.Client?.Client?.RemoteEndPoint as IPEndPoint)?.Address?.ToString() ?? "unknown", ConsoleFormatter.RedPastel, " disconnected.");
        private void Server_OnClientConnected(object sender, ClientConnectionEventArgs e) => ConsoleFormatter.DrawInfoMessageLine("Client at IP ", ConsoleFormatter.PurplePastel, (e?.Client?.Client?.RemoteEndPoint as IPEndPoint)?.Address?.ToString() ?? "unknown", ConsoleFormatter.GreenPastel, " connected.");
    }
}
