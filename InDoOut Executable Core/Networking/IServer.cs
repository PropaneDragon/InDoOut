using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace InDoOut_Executable_Core.Networking
{
    public interface IServer
    {
        bool Started { get; }
        int Port { get; }
        IReadOnlyCollection<TcpClient> Clients { get; }
        TimeSpan ClientPollInterval { get; set; }
        IPAddress IPAddress { get; }

        Task<bool> Start();
        Task<bool> Stop();
        Task<bool> SendMessageAll(string message);
        Task<bool> SendMessage(TcpClient client, string message);
    }
}