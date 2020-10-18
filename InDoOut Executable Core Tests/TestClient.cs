using InDoOut_Executable_Core.Networking;
using System.Threading;
using System.Threading.Tasks;

namespace InDoOut_Executable_Core_Tests
{
    internal class TestClient : Client
    {
        public INetworkMessage LastMessageReceived { get; set; } = null;
        public string LastRawMessageReceived { get; set; } = null;

        protected override Task<INetworkMessage> ProcessMessage(INetworkMessage message, CancellationToken cancellationToken)
        {
            LastMessageReceived = message;

            return base.ProcessMessage(message, cancellationToken);
        }

        protected override async Task MessageReceived(string message)
        {
            LastRawMessageReceived = message;

            await base.MessageReceived(message);
        }
    }
}
