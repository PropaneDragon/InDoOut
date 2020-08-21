using InDoOut_Executable_Core.Networking;
using System.Net.Sockets;

namespace InDoOut_Executable_Core_Tests
{
    internal class TestServer : AbstractServer
    {
        public TestServer(int port = 0) : base(port)
        {
        }

        public bool CanAcceptClients { get; set; } = true;
        public string LastMessageReceived { get; set; } = null;
        public TcpClient LastClientConnected { get; private set; } = null;
        public TcpClient LastClientDisconnected { get; private set; } = null;
        public TcpClient LastClientReceived { get; private set; } = null;

        protected override void ClientMessageReceived(TcpClient client, string message)
        {
            LastMessageReceived = message;
            LastClientReceived = client;
        }

        protected override bool CanAcceptClient(TcpClient client) => CanAcceptClients;
        protected override void ClientConnected(TcpClient client) => LastClientConnected = client;
        protected override void ClientDisconnected(TcpClient client) => LastClientDisconnected = client;
    }
}
