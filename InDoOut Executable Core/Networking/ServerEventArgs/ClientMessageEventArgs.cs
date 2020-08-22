using System;
using System.Net.Sockets;

namespace InDoOut_Executable_Core.Networking.ServerEventArgs
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
