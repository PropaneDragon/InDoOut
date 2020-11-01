namespace InDoOut_Executable_Core.Networking.Commands
{
    public interface ICommand
    {
        IInteractiveNetworkEntity BaseNetworkEntity { get; }
        string CommandName { get; }
    }

    public interface ICommand<NetworkEntityType> : ICommand where NetworkEntityType : class, IInteractiveNetworkEntity
    {
        NetworkEntityType NetworkEntity { get; set; }
    }
}