using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace InDoOut_Executable_Core.Networking.Commands
{
    public abstract class Command : ICommand
    {
        private string _inferredCommandName = null;

        public IInteractiveNetworkEntity BaseNetworkEntity { get; protected set; } = null;

        private Command()
        {
        }

        protected Command(IInteractiveNetworkEntity networkEntity) : this()
        {
            BaseNetworkEntity = networkEntity;
        }

        public virtual string CommandName => InferCommandNameUsingReflection();

        public override bool Equals(object obj) => obj is Command otherCommand ? otherCommand?.CommandName == CommandName : base.Equals(obj);
        public override int GetHashCode() => base.GetHashCode();

        protected async Task<bool> SendMessage(CancellationToken cancellationToken, params string[] data) => BaseNetworkEntity != null && await BaseNetworkEntity.SendMessage(CreateMessage(data), cancellationToken);
        protected async Task<bool> SendMessageWithContext(CancellationToken cancellationToken, object context, params string[] data) => BaseNetworkEntity != null && await BaseNetworkEntity.SendMessage(CreateMessageWithContext(context, data), cancellationToken);

        protected async Task<INetworkMessage> SendMessageAwaitResponse(CancellationToken cancellationToken, params string[] data) => BaseNetworkEntity != null ? await BaseNetworkEntity.SendMessageAwaitResponse(CreateMessage(data), cancellationToken) : null;
        protected async Task<INetworkMessage> SendMessageAwaitResponseWithContext(CancellationToken cancellationToken, object context, params string[] data) => BaseNetworkEntity != null ? await BaseNetworkEntity.SendMessageAwaitResponse(CreateMessageWithContext(context, data), cancellationToken) : null;

        protected NetworkMessage CreateMessage(params string[] data) => new(CommandName, data);
        protected NetworkMessage CreateMessageWithContext(object context, params string[] data) => new(CommandName, context, data);

        private string InferCommandNameUsingReflection()
        {
            if (_inferredCommandName == null)
            {
                _inferredCommandName = GetType().Name;
                _inferredCommandName = Regex.Replace(_inferredCommandName, @"([Ss]erver|[Cc]lient)[Cc]ommand", "");
            }

            return _inferredCommandName;
        }
    }

    public abstract class Command<NetworkEntityType> : Command, ICommand<NetworkEntityType> where NetworkEntityType : class, IInteractiveNetworkEntity
    {
        public NetworkEntityType NetworkEntity { get => BaseNetworkEntity as NetworkEntityType; set => BaseNetworkEntity = value; }

        public Command(NetworkEntityType networkEntity) : base(networkEntity)
        {
            NetworkEntity = networkEntity;
        }
    }
}
