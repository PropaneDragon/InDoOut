using System;
using System.Net.Sockets;

namespace InDoOut_Networking.Server.Events
{
    public class ClientMessageEventArgs : EventArgs
    {
        public string Message { get; private set; }
        public TcpClient Client { get; private set; }

        public ClientMessageEventArgs(TcpClient client, string message)
        {
            Client = client;
            Message = message;
        }
    }
}
