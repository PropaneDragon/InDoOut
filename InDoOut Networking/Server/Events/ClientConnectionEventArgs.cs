using System;
using System.Net.Sockets;

namespace InDoOut_Networking.Server.Events
{
    public class ClientConnectionEventArgs : EventArgs
    {
        public TcpClient Client { get; private set; }

        public ClientConnectionEventArgs(TcpClient client)
        {
            Client = client;
        }
    }
}
