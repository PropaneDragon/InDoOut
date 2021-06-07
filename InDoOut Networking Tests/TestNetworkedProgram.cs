using InDoOut_Networking.Client;
using InDoOut_Networking.Entities;

namespace InDoOut_Networking_Tests
{
    public class TestNetworkedProgram : NetworkedProgram
    {
        public TestNetworkedProgram(IClient client) : base(client)
        {
        }
    }
}
