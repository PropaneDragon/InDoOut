using InDoOut_Core.Entities.Functions;
using InDoOut_Networking.Shared.Entities;

namespace InDoOut_Networking.Entities
{
    public interface INetworkedInput : IInput
    {
        bool UpdateFromStatus(InputStatus status);
    }
}
