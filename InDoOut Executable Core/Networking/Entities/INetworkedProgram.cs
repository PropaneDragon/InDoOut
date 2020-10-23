using InDoOut_Core.Entities.Programs;

namespace InDoOut_Executable_Core.Networking.Entities
{
    public interface INetworkedProgram : IProgram
    {
        bool Connected { get; }
    }
}