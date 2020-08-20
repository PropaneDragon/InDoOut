namespace InDoOut_Executable_Core.Networking
{
    public class ProgramSyncCommand
    {
        private static readonly string COMMAND_SEPARATOR = "\u0004\u0001\u0004";

        public bool Valid => !string.IsNullOrWhiteSpace(Command);
        public string Command { get; set; } = null;
        public string Data { get; set; } = null;
        public string FullCommandString => Valid ? $"{Command}{COMMAND_SEPARATOR}{Data ?? ""}" : null;

        private ProgramSyncCommand()
        {
        }

        public ProgramSyncCommand(string command, string data = null)
        {
            Command = command;
            Data = data;
        }

        public static ProgramSyncCommand ExtractFromCommandString(string fullCommandString)
        {
            if (!string.IsNullOrWhiteSpace(fullCommandString))
            {
                var split = fullCommandString.Split(COMMAND_SEPARATOR);
                if (split.Length == 2)
                {
                    return new ProgramSyncCommand(split[0], split[1]);
                }
            }

            return null;
        }
    }
}
