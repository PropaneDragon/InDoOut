using InDoOut_Executable_Core.Networking;

namespace InDoOut_Executable_Core_Tests
{
    internal class TestClient : AbstractClient
    {
        public string LastMessageReceived { get; set; } = null;

        protected override void MessageReceived(string message) => LastMessageReceived = message;
    }
}
