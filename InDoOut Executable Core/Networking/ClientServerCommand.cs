namespace InDoOut_Executable_Core.Networking
{
    public class ClientServerCommand : ICommand
    {
        public bool Valid => !string.IsNullOrWhiteSpace(Name);
        public string ErrorMessage => ExtractErrorFromData();
        public string SuccessMessage => ExtractSuccessFromData();
        public string Name { get; set; } = null;
        public string Data { get; set; } = null;
        public string FullCommandString => Valid ? $"{Name}{NetworkCodes.COMMAND_NAME_DATA_SPLITTER}{Data ?? ""}" : null;

        private ClientServerCommand()
        {
        }

        public ClientServerCommand(string command, params string[] data)
        {
            Name = command;
            Data = string.Join(NetworkCodes.COMMAND_DATA_GENERIC_SPLITTER, data);
        }

        public static ClientServerCommand FromCommandString(string fullCommandString)
        {
            if (!string.IsNullOrWhiteSpace(fullCommandString))
            {
                var split = fullCommandString.Split(NetworkCodes.COMMAND_NAME_DATA_SPLITTER);
                if (split.Length == 2)
                {
                    return new ClientServerCommand(split[0], split[1]);
                }
            }

            return null;
        }

        public static ClientServerCommand CreateErrorReply(ClientServerCommand replyToCommand, string errorMessage = null) => replyToCommand != null ? new ClientServerCommand(replyToCommand.Name, $"{NetworkCodes.COMMAND_FAILURE_IDENTIFIER}:{errorMessage}") : null;
        public static ClientServerCommand CreateSuccessReply(ClientServerCommand replyToCommand, string successMessage = null) => replyToCommand != null ? new ClientServerCommand(replyToCommand.Name, $"{NetworkCodes.COMMAND_SUCCESS_IDENTIFIER}:{successMessage}") : null;

        private string ExtractErrorFromData() => !string.IsNullOrWhiteSpace(Data) && Data.StartsWith(NetworkCodes.COMMAND_FAILURE_IDENTIFIER) ? Data.Replace($"{NetworkCodes.COMMAND_FAILURE_IDENTIFIER}:", "") : null;
        private string ExtractSuccessFromData() => !string.IsNullOrWhiteSpace(Data) && Data.StartsWith(NetworkCodes.COMMAND_SUCCESS_IDENTIFIER) ? Data.Replace($"{NetworkCodes.COMMAND_SUCCESS_IDENTIFIER}:", "") : null;
    }
}
