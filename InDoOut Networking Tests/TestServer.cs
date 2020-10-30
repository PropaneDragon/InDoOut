using InDoOut_Executable_Core.Networking;
using InDoOut_Networking.Server;
using InDoOut_Networking.Server.Events;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace InDoOut_Networking_Tests
{
    internal class TestServer : Server
    {
        public TestServer(int port = 0) : base(port)
        {
            OnClientConnected += TestServer_OnClientConnected;
            OnClientDisconnected += TestServer_OnClientDisconnected;
            OnClientMessageReceived += TestServer_OnClientMessageReceived;
        }

        public bool CanAcceptClients { get; set; } = true;
        public string LastRawMessageReceived { get; set; } = null;
        public INetworkMessage LastMessageReceived { get; set; } = null;
        public TcpClient LastClientConnected { get; private set; } = null;
        public TcpClient LastClientDisconnected { get; private set; } = null;
        public TcpClient LastClientReceived { get; private set; } = null;

        protected override Task<INetworkMessage> ProcessMessage(INetworkMessage message, CancellationToken cancellationToken)
        {
            LastMessageReceived = message;

            return base.ProcessMessage(message, cancellationToken);
        }

        private void TestServer_OnClientMessageReceived(object sender, ClientMessageEventArgs e)
        {
            LastRawMessageReceived = e.Message;
            LastClientReceived = e.Client;
        }

        private void TestServer_OnClientDisconnected(object sender, ClientConnectionEventArgs e) => LastClientDisconnected = e.Client;
        private void TestServer_OnClientConnected(object sender, ClientConnectionEventArgs e) => LastClientConnected = e.Client;
    }
}
