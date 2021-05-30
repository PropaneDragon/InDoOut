using InDoOut_Core.Logging;
using InDoOut_Executable_Core.Networking.Commands;
using System.Threading;
using System.Threading.Tasks;

namespace InDoOut_Executable_Core.Networking
{
    public interface IInteractiveNetworkEntity
    {
        ILog EntityLog { get; }

        bool AddCommandListener(ICommandListener listener);
        Task<bool> SendMessage(INetworkMessage command, CancellationToken cancellationToken);
        Task<INetworkMessage> SendMessageAwaitResponse(INetworkMessage command, CancellationToken cancellationToken);
    }
}
