using InDoOut_Console_Common.ConsoleExtensions;
using InDoOut_Core.Entities.Programs;
using InDoOut_Core.Functions;
using InDoOut_Core.Reporting;
using InDoOut_Executable_Core.Location;
using InDoOut_Executable_Core.Storage;
using InDoOut_Json_Storage;
using InDoOut_Plugins.Loaders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace InDoOut_Console_Common.Loading
{
    public class ConsoleProgramLoader : ConsoleLoader
    {
        public string ProgramLocation { get; set; } = null;
        public string[] Arguments { get; set; } = null;

        public IProgram LoadedProgram { get; private set; } = null;

        public override string Name => "loading program";

        public ConsoleProgramLoader()
        {
        }

        public ConsoleProgramLoader(string programLocation, params string[] arguments)
        {
            ProgramLocation = programLocation;
            Arguments = arguments;
        }

        protected override bool BeginLoad()
        {
            if (!string.IsNullOrEmpty(ProgramLocation))
            {
                try
                {
                    using var fileStream = new FileStream(ProgramLocation, FileMode.Open, FileAccess.Read);

                    var program = new Program(Arguments);
                    var storage = new ProgramJsonStorer(new FunctionBuilder(), LoadedPlugins.Instance);
                    var hasLoaded = WriteMessageLine(() => LoadProgram(program, storage, fileStream), "Attempting to load program from ", ConsoleFormatter.AccentTertiary, ProgramLocation, ConsoleFormatter.Primary, "...");

                    if (hasLoaded)
                    {
                        _ = StandardLocations.Instance.SetPathTo(Location.SaveFile, ProgramLocation);

                        LoadedProgram = program;
                    }

                    return hasLoaded;
                }
                catch (Exception ex)
                {
                    ConsoleFormatter.DrawErrorMessageLine("Couldn't load program due to a file error: ", ex?.Message);
                }
            }
            else
            {
                ConsoleFormatter.DrawErrorMessageLine("Couldn't load program as it was empty.");
            }

            return false;
        }

        private bool LoadProgram(IProgram program, IProgramStorer storage, Stream stream)
        {
            if (program != null && storage != null)
            {
                var failureReports = storage.Load(program, stream);

                PrintFailures(failureReports);

                if (!failureReports.Any(report => report.Critical))
                {
                    if (failureReports.Count > 0)
                    {
                        ConsoleFormatter.DrawWarningMessageLine("Program loaded, however some of the information couldn't be fully loaded and the program might not work properly. See above for details.");
                        ConsoleFormatter.DrawWarningMessageLine("Press any key to start anyway...");

                        _ = Console.ReadKey();
                    }

                    ConsoleFormatter.DrawInfoMessageLine(ConsoleFormatter.GreenPastel, "Program loaded.");

                    if (program.StartFunctions.Count > 0)
                    {
                        ConsoleFormatter.DrawInfoMessageLine("Starting program...");

                        program.Trigger(null);

                        if (program.Running)
                        {
                            ConsoleFormatter.DrawInfoMessageLine(ConsoleFormatter.GreenPastel, "Program running.");
                        }

                        return true;
                    }
                    else
                    {
                        ConsoleFormatter.DrawErrorMessageLine("The program cannot be started. Please ensure the program has start blocks to ensure there is an entry point into the program.");
                    }
                }
                else
                {
                    ConsoleFormatter.DrawErrorMessageLine("Load aborted due to critical errors documented above. Please fix and retry.");
                }
            }
            else
            {
                ConsoleFormatter.DrawErrorMessageLine("Couldn't start due to an internal error: Invalid program or storage.");
            }

            return false;
        }

        private void PrintFailures(List<IFailureReport> failureReports)
        {
            if (failureReports.Count > 0)
            {
                ExtendedConsole.WriteLine();
                ConsoleFormatter.DrawCustomHeader(ConsoleFormatter.Primary, ConsoleFormatter.Orange, 1, 1, 1, 1, "The following errors have occurred during load:");

                foreach (var report in failureReports)
                {
                    if (report.Critical)
                    {
                        ConsoleFormatter.DrawErrorMessageLine(report.Summary);
                    }
                    else
                    {
                        ConsoleFormatter.DrawWarningMessageLine(report.Summary);
                    }
                }

                ExtendedConsole.WriteLine();
            }
        }
    }
}
