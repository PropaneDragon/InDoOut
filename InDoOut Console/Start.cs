using InDoOut_Console.Arguments;
using InDoOut_Console.Display;
using InDoOut_Console.Loading;
using InDoOut_Console.Messaging;
using InDoOut_Console_Common.ConsoleExtensions;
using InDoOut_Core.Logging;
using InDoOut_Executable_Core.Arguments;
using InDoOut_Executable_Core.Location;
using InDoOut_Executable_Core.Logging;
using InDoOut_Executable_Core.Messaging;
using System;
using System.IO;
using System.Linq;

namespace InDoOut_Console
{
    class Start
    {
        private static bool _keepOpen = true;
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
                _keepOpen = !applicationArguments.AutoClose;

                ExtendedConsole.HighColourMode = !applicationArguments.LegacyConsoleMode;

                ConsoleFormatter.DrawTitle("in > do > out");
                ConsoleFormatter.DrawSubtitle("Console program runner");

                ExtendedConsole.WriteLine();

                if (applicationArguments.ShouldShowHelp)
                {
                    _keepOpen = true;

                    applicationArguments.DisplayHelp();
                }
                else
                {
                    var pluginLoader = new ConsolePluginLoader();

                    if (pluginLoader.LoadPlugins())
                    {
                        if (!string.IsNullOrEmpty(applicationArguments.ProgramToLoad))
                        {
                            var rawProgramName = Path.GetFileNameWithoutExtension(applicationArguments.ProgramToLoad);

                            if (!string.IsNullOrWhiteSpace(rawProgramName))
                            {
                                var logFileName = $"IDO-{rawProgramName}.log";

                                ConsoleFormatter.DrawInfoMessageLine("Changing log file name to output to ", ConsoleFormatter.PurplePastel, logFileName);

                                _logFileSaver.LogFileName = logFileName;

                                var programLoader = new ConsoleProgramLoader();
                                var program = programLoader.LoadProgram(applicationArguments.ProgramToLoad, applicationArguments.LaodedProgramArguments.ToArray());

                                if (program != null)
                                {
                                    var programDisplay = new ProgramRunDisplay();
                                    _ = programDisplay.StartProgramDisplay(program);
                                }
                            }
                        }
                        else
                        {
                            ConsoleFormatter.DrawErrorMessageLine("No program given to load! Please provide a program using the ", ConsoleFormatter.YellowPastel, "-", ConsoleFormatter.PurplePastel, "program", ConsoleFormatter.Primary, " command.");
                        }
                    }
                    else
                    {
                        ConsoleFormatter.DrawErrorMessageLine("One or more plugins failed to load successfully. Please check the errors above.");
                    }
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
            WaitToComplete();

            _ = _logFileSaver?.SaveLog();
        }

        private static void WaitToComplete()
        {
            Log.Instance.Header("Console application waiting for user to finish");

            if (_keepOpen)
            {
                ExtendedConsole.WriteLine();

                ConsoleFormatter.DrawCustomHeader(ConsoleFormatter.Highlight, ConsoleFormatter.Green, 1, 1, 1, 1, "Completed. Press any key to close.");

                _ = Console.ReadKey();
            }

            Log.Instance.Header("Console application finished");
        }
    }
}
