using InDoOut_Core.Entities.Programs;
using InDoOut_Networking.Client;
using InDoOut_Networking.Entities;
using InDoOut_Networking.Shared.Entities;

namespace InDoOut_Networking_Tests
{
    public class TestNetworkedProgram : NetworkedProgram
    {
        public TestNetworkedProgram(IClient client) : base(client)
        {
        }

        public TestNetworkedProgram(IClient client, IProgram program) : base(client, program)
        {
        }

        public bool UpdateFromProgramStatusPublic(ProgramStatus status) => UpdateFromProgramStatus(status);
    }
}
