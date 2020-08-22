using InDoOut_Console_Common.Arguments;
using InDoOut_Core.Entities.Functions;
using InDoOut_Executable_Core.Arguments;
using System.Collections.Generic;

namespace InDoOut_Console.Arguments
{
    public class ApplicationArguments
    {
        private readonly List<string> _loadedProgramArguments = new List<string>();
        private readonly ConsoleHelpArgument _helpArgument = new ConsoleHelpArgument(false);

        public bool ShouldShowHelp { get; private set; } = false;
        public bool LegacyConsoleMode { get; private set; } = false;
        public bool AutoClose { get; private set; } = false;
        public string ProgramToLoad { get; private set; } = null;

        public IReadOnlyCollection<string> LaodedProgramArguments => _loadedProgramArguments.AsReadOnly();

        public bool ProcessArguments(string[] args)
        {
            var legacyConsoleArgument = new BasicArgument("legacyConsole", "Starts in low colour mode for old console windows.", false, false, (handler, value) => LegacyConsoleMode = true);
            var programArgument = new BasicArgument("program", "The path to the program to start.", "", false, (handler, value) => ProgramToLoad = value);
            var autoCloseArgument = new BasicArgument("autoClose", "Immediately closes the application after the running program has completed.", false, false, (handler, value) => AutoClose = true);
            var programRunArguments = new List<ConsoleProgramRunArgument>();

            _ = ArgumentHandler.Instance.AddArgument(_helpArgument);
            _ = ArgumentHandler.Instance.AddArgument(programArgument);
            _ = ArgumentHandler.Instance.AddArgument(legacyConsoleArgument);
            _ = ArgumentHandler.Instance.AddArgument(autoCloseArgument);

            for (var id = 1; id <= StartFunction.TOTAL_OUTPUTS; ++id)
            {
                var runArgument = new ConsoleProgramRunArgument(id);
                programRunArguments.Add(runArgument);
                _ = ArgumentHandler.Instance.AddArgument(runArgument);
            }

            ArgumentHandler.Instance.Process(args);

            foreach (var runArgument in programRunArguments)
            {
                _loadedProgramArguments.Add(runArgument.Value);
            }

            ShouldShowHelp = _helpArgument.ShouldShowHelp;

            return true;
        }

        public void DisplayHelp()
        {
            _helpArgument.ShowHelp(ArgumentHandler.Instance);
        }
    }
}
