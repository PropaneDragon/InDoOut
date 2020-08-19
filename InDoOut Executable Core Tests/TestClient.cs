using InDoOut_Executable_Core.Networking;

namespace InDoOut_Executable_Core_Tests
{
    internal class TestClient : Client
    {
        public string LastMessageReceived { get; private set; } = null;

        protected override void MessageReceived(string message) => LastMessageReceived = message;
    }
}
