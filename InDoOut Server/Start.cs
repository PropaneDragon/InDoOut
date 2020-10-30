using InDoOut_Console_Common.ConsoleExtensions;
using InDoOut_Console_Common.Messaging;
using InDoOut_Executable_Core.Arguments;
using InDoOut_Executable_Core.Location;
using InDoOut_Executable_Core.Logging;
using InDoOut_Executable_Core.Messaging;
using InDoOut_Server.Arguments;
using InDoOut_Server.ServerNetworking;
using System;

namespace InDoOut_Server
{
    class Start
    {
        private static LogFileSaver _logFileSaver = null;
        private static ConsoleServerManager _serverManager = null;

        static void Main(string[] args)
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

                    _serverManager = new ConsoleServerManager(arguments.ChosenPort);
                    if (_serverManager.Start())
                    {
                        ConsoleFormatter.DrawSubtitle("Server started");
                    }

                    _ = Console.ReadKey();
                }
            }

            TearDown();
        }

        private static void SetUp()
        {
            UserMessageSystemHolder.Instance.CurrentUserMessageSystem = new ConsoleUserMessageSystem();

            _logFileSaver = new LogFileSaver(StandardLocations.Instance);
            _logFileSaver.BeginAutoSave();
        }

        private static void TearDown()
        {
            _ = _logFileSaver?.SaveLog();
        }

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
