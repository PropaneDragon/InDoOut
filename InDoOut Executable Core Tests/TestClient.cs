using InDoOut_Executable_Core.Networking;

namespace InDoOut_Executable_Core_Tests
{
    internal class TestClient : Client
    {
        public string LastMessageReceived { get; private set; } = null;

        protected override void MessageReceived(string message)
        {
            if (message != "\u0001\u0001\u0003") //Todo: Make this pass properly. This is the alive check sent by the server and needs integrating properly.
            {
                LastMessageReceived = message;
            }
        }
    }
}
