using System.Threading;
using System.Threading.Tasks;

namespace InDoOut_Executable_Core.Networking.Commands
{
    public interface ICommandListener : ICommand
    {
        Task<INetworkMessage> CommandReceived(INetworkMessage message, CancellationToken cancellationToken);
    }

    public interface ICommandListener<NetworkEntityType> : ICommand<NetworkEntityType>, ICommandListener where NetworkEntityType : class, IInteractiveNetworkEntity
    {

    }
}
