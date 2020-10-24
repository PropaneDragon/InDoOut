namespace InDoOut_Executable_Core.Networking
{
    public interface INetworkMessage
    {
        bool Valid { get; }
        bool IsFailureMessage { get; }
        bool IsSuccessMessage { get; }

        string Id { get; }
        string Name { get; set; }
        string[] Data { get; set; }

        string FailureMessage { get; }
        string SuccessMessage { get; }

        object Context { get; set; }

        INetworkMessage CreateResponseMessage(string[] data);
        INetworkMessage CreateSuccessResponse(string message = null);
        INetworkMessage CreateFailureResponse(string message = null);
    }
}