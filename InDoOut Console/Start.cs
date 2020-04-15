using InDoOut_Console.Arguments;
using InDoOut_Console.Display;
using InDoOut_Console.Messaging;
using InDoOut_Console.ProgramView;
using InDoOut_Core.Entities.Programs;
using InDoOut_Core.Functions;
using InDoOut_Core.Logging;
using InDoOut_Core.Reporting;
using InDoOut_Executable_Core.Arguments;
using InDoOut_Executable_Core.Location;
using InDoOut_Executable_Core.Logging;
using InDoOut_Executable_Core.Messaging;
using InDoOut_Executable_Core.Storage;
using InDoOut_Function_Plugins.Loaders;
using InDoOut_Json_Storage;
using InDoOut_Plugins.Loaders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace InDoOut_Console
{
    class Start
    {
        private static string _fileToOpen = null;
        private static LogFileSaver _logFileSaver = null;

        static void Main(string[] args)
        {
            if (ProcessArguments(args))
            {
                Log.Instance.Header("Console application started");

                SetUp();
                WriteHeader();
                LoadPlugins();

                if (!string.IsNullOrEmpty(_fileToOpen))
                {
                    LoadProgram(_fileToOpen);
                }
                else
                {
                    ColourConsole.WriteErrorLine("No program to load.");
                }
            }

            WaitToComplete();

            _ = _logFileSaver?.SaveLog();
        }

        private static bool ProcessArguments(string[] args)
        {
            var helpArgument = new ConsoleHelpArgument();
            var programArgument = new BasicArgument("program", "The path to the program to start.", "", false, (handler, value) => _fileToOpen = value);

            _ = ArgumentHandler.Instance.AddArgument(helpArgument);
            _ = ArgumentHandler.Instance.AddArgument(programArgument);

            ArgumentHandler.Instance.Process(args);

            return !helpArgument.HelpShown;
        }

        private static void SetUp()
        {
            UserMessageSystemHolder.Instance.CurrentUserMessageSystem = new ConsoleUserMessageSystem();

            _logFileSaver = new LogFileSaver(StandardLocations.Instance);
            _logFileSaver.BeginAutoSave();
        }

        private static void WaitToComplete()
        {
            Log.Instance.Header("Console application waiting for user to finish");

            Console.WriteLine();
            ColourConsole.WriteInfo("Press any key to close... ");

            _ = Console.ReadKey();

            Log.Instance.Header("Console application finished");
        }

        private static void LoadProgram(string programToStart)
        {
            if (!string.IsNullOrEmpty(programToStart))
            {
                ColourConsole.WriteInfoLine(new ColourBlock("Attempting to load program at "), new ColourBlock(programToStart, ConsoleColor.Yellow), new ColourBlock("..."));

                _logFileSaver.LogFileName = $"IDO-{Path.GetFileNameWithoutExtension(programToStart)}.log";

                var program = new Program();
                var storage = new ProgramJsonStorer(new FunctionBuilder(), LoadedPlugins.Instance, programToStart);

                LoadProgram(program, storage);
            }
            else
            {
                ColourConsole.WriteErrorLine("Program to load is empty.");
            }
        }

        private static void LoadProgram(IProgram program, IProgramStorer storage)
        {
            if (program != null && storage != null)
            {
                var failureReports = storage.Load(program);

                PrintFailures(failureReports);

                if (!failureReports.Any(report => report.Critical))
                {
                    if (failureReports.Count > 0)
                    {
                        Console.WriteLine();
                        ColourConsole.WriteWarningLine("No critical errors, however program might be missing important information due to load errors documented above.", ConsoleColor.Red);
                        ColourConsole.WriteWarning("Press any key to start regardless... ", ConsoleColor.Red);
                        _ = Console.ReadKey();
                        Console.WriteLine();
                        Console.WriteLine();
                    }

                    ColourConsole.WriteInfoLine("Program loaded successfully.", ConsoleColor.Green);

                    _ = StandardLocations.Instance.SetPathTo(Location.SaveFile, storage.FilePath);

                    if (program.StartFunctions.Count > 0)
                    {
                        ColourConsole.WriteInfoLine("Starting program...");

                        program.Trigger(null);

                        Thread.Sleep(TimeSpan.FromMilliseconds(10));

                        if (program.Running)
                        {
                            ColourConsole.WriteInfoLine("Program running.", ConsoleColor.Green);
                        }

                        var display = new ProgramDisplay(program);
                        display.ShowRunStatus();

                        ColourConsole.WriteInfoLine("Program complete.", ConsoleColor.Green);
                    }
                    else
                    {
                        ColourConsole.WriteErrorLine("The program cannot be started. Please ensure the program has start blocks to ensure there is an entry point into the program.");
                    }
                }
                else
                {
                    Console.WriteLine();
                    ColourConsole.WriteErrorLine("Load aborted due to critical errors documented above. Please fix and retry.", ConsoleColor.Red);
                }
            }
            else
            {
                Console.WriteLine();
                ColourConsole.WriteErrorLine("Couldn't start due to an internal error: Invalid program or storage.", ConsoleColor.Red);
            }
        }

        private static void PrintFailures(List<IFailureReport> failureReports)
        {
            if (failureReports.Count > 0)
            {
                ColourConsole.WriteErrorLine($"The following errors have occurred during load:");

                foreach (var report in failureReports)
                {
                    if (report.Critical)
                    {
                        ColourConsole.WriteErrorLine(report.Summary, ConsoleColor.Red);
                    }
                    else
                    {
                        ColourConsole.WriteWarningLine(report.Summary, ConsoleColor.DarkYellow);
                    }
                }
            }
        }

        private static void LoadPlugins()
        {
            ColourConsole.WriteInfoLine("Loading plugins...");

            var pluginLoader = new PluginDirectoryLoader(new FunctionPluginLoader(), StandardLocations.Instance);
            var loadedPlugins = pluginLoader.LoadPlugins().Result;

            foreach (var plugin in loadedPlugins)
            {
                ColourConsole.WriteInfoLine(new ColourBlock("Loading plugin "), new ColourBlock($"{plugin?.Plugin?.SafeName ?? "Invalid plugin"}", ConsoleColor.Yellow), new ColourBlock("..."));

                if (plugin.Initialise())
                {
                    ColourConsole.WriteInfoLine("Loaded.", ConsoleColor.Green);
                }
                else
                {
                    ColourConsole.WriteErrorLine("Failed to load.");
                }

                Console.WriteLine();
            }

            LoadedPlugins.Instance.Plugins = loadedPlugins;
        }

        private static void WriteHeader()
        {
            var originalBackgroundColour = Console.BackgroundColor;

            Console.BackgroundColor = ConsoleColor.DarkGray;
            Console.WriteLine();

            ColourConsole.WriteLine(new ColourBlock("in", ConsoleColor.Red), new ColourBlock(" > ", ConsoleColor.Gray), new ColourBlock("do", ConsoleColor.Red), new ColourBlock(" > ", ConsoleColor.Gray), new ColourBlock("out", ConsoleColor.Red));

            Console.WriteLine();
            Console.BackgroundColor = originalBackgroundColour;
        }
    }
}
