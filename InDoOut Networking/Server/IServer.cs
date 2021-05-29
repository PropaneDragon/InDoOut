using InDoOut_Core.Logging;
using InDoOut_Executable_Core.Networking;
using InDoOut_Networking.Server.Events;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace InDoOut_Networking.Server
{
    public interface IServer : IInteractiveNetworkEntity
    {
        bool Started { get; }
        int Port { get; }
        ILog DisplayLog { get; }
        IReadOnlyCollection<TcpClient> Clients { get; }
        TimeSpan ClientPollInterval { get; set; }
        IPAddress IPAddress { get; }

        event EventHandler<ServerConnectionEventArgs> OnServerStarted;
        event EventHandler<ServerConnectionEventArgs> OnServerStopped;
        event EventHandler<ClientConnectionEventArgs> OnClientConnected;
        event EventHandler<ClientConnectionEventArgs> OnClientDisconnected;
        event EventHandler<ClientMessageEventArgs> OnClientMessageReceived;
        event EventHandler<ClientMessageEventArgs> OnClientMessageSent;

        Task<bool> Start();
        Task<bool> Stop();
        Task<bool> SendMessageAll(string message);
        Task<bool> SendMessage(TcpClient client, string message);
    }
}