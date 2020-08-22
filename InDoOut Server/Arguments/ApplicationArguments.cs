using InDoOut_Console_Common.Arguments;
using InDoOut_Executable_Core.Arguments;

namespace InDoOut_Server.Arguments
{
    public class ApplicationArguments
    {
        private readonly ConsoleHelpArgument _helpArgument = new ConsoleHelpArgument(false);

        public bool LegacyConsoleMode { get; private set; } = false;
        public int ChosenPort { get; private set; } = 0;

        public bool ProcessArguments(string[] args)
        {
            var legacyConsoleArgument = new BasicArgument("legacyConsole", "Starts in low colour mode for old console windows.", false, false, (handler, value) => LegacyConsoleMode = true);
            var portArgument = new BasicArgument("port", "Starts the server with the given port instead of picking any available port automatically.", "0", false, (handler, value) => ChosenPort = int.TryParse(value, out var port) ? port : 0);

            _ = ArgumentHandler.Instance.AddArgument(_helpArgument);
            _ = ArgumentHandler.Instance.AddArgument(portArgument);
            _ = ArgumentHandler.Instance.AddArgument(legacyConsoleArgument);

            ArgumentHandler.Instance.Process(args);

            return true;
        }

        public void DisplayHelp()
        {
            _helpArgument.ShowHelp(ArgumentHandler.Instance);
        }
    }
}
