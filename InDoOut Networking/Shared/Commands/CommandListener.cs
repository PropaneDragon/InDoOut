using System.Threading;
using System.Threading.Tasks;

namespace InDoOut_Executable_Core.Networking.Commands
{
    public abstract class CommandListener<NetworkEntityType> : Command<NetworkEntityType>, ICommandListener<NetworkEntityType> where NetworkEntityType : class, IInteractiveNetworkEntity
    {
        protected CommandListener(NetworkEntityType networkEntity) : base(networkEntity)
        {
        }

        public abstract Task<INetworkMessage> CommandReceived(INetworkMessage message, CancellationToken cancellationToken);
    }
}
