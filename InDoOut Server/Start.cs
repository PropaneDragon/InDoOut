using InDoOut_Console_Common.Commands;
using InDoOut_Console_Common.ConsoleExtensions;
using InDoOut_Console_Common.Messaging;
using InDoOut_Core.Logging;
using InDoOut_Executable_Core.Arguments;
using InDoOut_Executable_Core.Location;
using InDoOut_Executable_Core.Logging;
using InDoOut_Executable_Core.Messaging;
using InDoOut_Server.Arguments;
using InDoOut_Server.ServerNetworking;

namespace InDoOut_Server
{
    public class Start
    {
        private static Log _serverLog = null, _eventLog = null;
        private static LogFileSaver _logFileSaver = null;
        private static ConsoleServerManager _serverManager = null;

        public static void Main(string[] args)
        {
            ExtendedConsole.SetUp();

            SetUp();

            var arguments = ProcessArguments(args);
            if (arguments != null)
            {
                ExtendedConsole.HighColourMode = !arguments.LegacyConsoleMode;

                ConsoleFormatter.DrawTitle("in > do > out");
                ConsoleFormatter.DrawSubtitle("Server");

                ExtendedConsole.WriteLine();

                if (arguments.ShouldShowHelp)
                {
                    arguments.DisplayHelp();
                }
                else
                {
                    ConsoleFormatter.DrawSubtitle("Starting server");

                    _serverManager = new ConsoleServerManager(_serverLog, _eventLog, arguments.ChosenPort);
                    if (_serverManager.Start())
                    {
                        ConsoleFormatter.DrawSubtitle("Server started");
                    }

                    var commandHandler = new CommandHandler();
                    commandHandler.AddCommand(new ExitCommand(commandHandler));
                    commandHandler.AddCommand(new LogCommand(_serverLog, _eventLog, Log.Instance));

                    _ = commandHandler.AwaitCommands();

                    ConsoleFormatter.DrawSubtitle("Closing server");
                }
            }

            TearDown();
        }

        private static void SetUp()
        {
            UserMessageSystemHolder.Instance.CurrentUserMessageSystem = new ConsoleUserMessageSystem();

            _serverLog = new("Server log");
            _eventLog = new("Events");

            _logFileSaver = new LogFileSaver(StandardLocations.Instance);
            _logFileSaver.BeginAutoSave();
        }

        private static void TearDown() => _ = _logFileSaver?.SaveLog();

        private static ApplicationArguments ProcessArguments(string[] args)
        {
            var applicationArguments = new ApplicationArguments();

            try
            {
                if (applicationArguments.ProcessArguments(args))
                {
                    return applicationArguments;
                }
            }
            catch (InvalidArgumentException ex)
            {
                ExtendedConsole.HighColourMode = false;

                ConsoleFormatter.DrawErrorMessageLine(ex.Message);
            }

            return null;
        }
    }
}
