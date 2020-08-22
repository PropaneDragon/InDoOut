using InDoOut_Console_Common.ConsoleExtensions;
using InDoOut_Console_Common.Messaging;
using InDoOut_Executable_Core.Arguments;
using InDoOut_Executable_Core.Location;
using InDoOut_Executable_Core.Logging;
using InDoOut_Executable_Core.Messaging;
using InDoOut_Server.Arguments;
using InDoOut_Server.ServerNetworking;
using System;
using System.Threading;

namespace InDoOut_Server
{
    class Start
    {
        private static LogFileSaver _logFileSaver = null;

        static void Main(string[] args)
        {
            ExtendedConsole.SetUp();

            SetUp();

            var applicationArguments = new ApplicationArguments();
            var processedArguments = false;

            try
            {
                processedArguments = applicationArguments.ProcessArguments(args);
            }
            catch (InvalidArgumentException ex)
            {
                ExtendedConsole.HighColourMode = false;

                ConsoleFormatter.DrawErrorMessageLine(ex.Message);
            }

            if (processedArguments)
            {
                ExtendedConsole.HighColourMode = !applicationArguments.LegacyConsoleMode;

                ConsoleFormatter.DrawTitle("in > do > out");
                ConsoleFormatter.DrawSubtitle("Server");

                ExtendedConsole.WriteLine();

                ConsoleFormatter.DrawSubtitle("Starting server");

                var serverManager = new ConsoleServerManager(applicationArguments.ChosenPort);
                serverManager.Start();

                Thread.Sleep(TimeSpan.FromSeconds(1000));
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
    }
}
