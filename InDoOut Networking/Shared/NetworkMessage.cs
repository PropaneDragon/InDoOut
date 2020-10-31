using System;

namespace InDoOut_Executable_Core.Networking
{
    public class NetworkMessage : INetworkMessage
    {
        public bool Valid => !string.IsNullOrWhiteSpace(Id) && !string.IsNullOrWhiteSpace(Name);

        public bool IsFailureMessage => Data != null && Data.Length > 0 && Data[0] == NetworkCodes.COMMAND_FAILURE_IDENTIFIER;
        public bool IsSuccessMessage => Data != null && Data.Length > 0 && Data[0] == NetworkCodes.COMMAND_SUCCESS_IDENTIFIER;

        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = null;
        public string[] Data { get; set; } = null;

        public string FailureMessage => IsFailureMessage && Data.Length > 1 ? string.Join("", Data[1..]) : null;
        public string SuccessMessage => IsSuccessMessage && Data.Length > 1 ? string.Join("", Data[1..]) : null;

        public object Context { get; set; } = null;

        public NetworkMessage()
        {
        }

        public NetworkMessage(string name, params string[] data) : this()
        {
            Name = name;
            Data = data;
        }

        public NetworkMessage(string name, object context, params string[] data) : this(name, data)
        {
            Context = context;
        }

        public static NetworkMessage FromString(string @string)
        {
            if (!string.IsNullOrEmpty(@string))
            {
                var idSplit = @string.Split(NetworkCodes.MESSAGE_ID_COMMAND_SPLITTER);
                if (idSplit.Length > 1)
                {
                    var id = idSplit[0];
                    var remainingString = string.Join("", idSplit[1..]);
                    var nameDataSplit = remainingString.Split(NetworkCodes.COMMAND_NAME_DATA_SPLITTER);
                    
                    if (nameDataSplit.Length > 0)
                    {
                        var name = nameDataSplit[0];
                        string[] data = null;

                        if (nameDataSplit.Length > 1)
                        {
                            remainingString = string.Join("", nameDataSplit[1..]);
                            if (!string.IsNullOrEmpty(remainingString))
                            {
                                data = remainingString.Split(NetworkCodes.COMMAND_DATA_GENERIC_SPLITTER);
                            }
                        }

                        if (!string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(name))
                        {
                            return new NetworkMessage(name, data) { Id = id };
                        }
                    }
                }
            }

            return null;
        }

        public INetworkMessage CreateSuccessResponse(string message) => CreateResponseMessage(new string[] { NetworkCodes.COMMAND_SUCCESS_IDENTIFIER, message ?? "" });
        public INetworkMessage CreateFailureResponse(string message) => CreateResponseMessage(new string[] { NetworkCodes.COMMAND_FAILURE_IDENTIFIER, message ?? "" });
        public INetworkMessage CreateResponseMessage(params string[] data) => new NetworkMessage(Name, data) { Id = Id, Context = Context };

        public override string ToString() => Valid ? $"{Id}{NetworkCodes.MESSAGE_ID_COMMAND_SPLITTER}{Name}{NetworkCodes.COMMAND_NAME_DATA_SPLITTER}{CompressedData ?? ""}" : null;

        private string CompressedData => Data != null ? string.Join(NetworkCodes.COMMAND_DATA_GENERIC_SPLITTER, Data) : "";
    }
}
