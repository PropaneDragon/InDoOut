using InDoOut_Core.Entities.Functions;
using InDoOut_Networking.Shared.Entities;

namespace InDoOut_Networking.Entities
{
    public interface INetworkedOutput : IOutput
    {
        bool UpdateFromStatus(OutputStatus status);
    }
}
