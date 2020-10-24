using InDoOut_Core.Entities.Programs;
using System.Threading.Tasks;

namespace InDoOut_Executable_Core.Networking.Entities
{
    public interface INetworkedProgram : IProgram
    {
        bool Connected { get; }

        IClient AssociatedClient { get; }

        Task<bool> Disconnect();
    }
}