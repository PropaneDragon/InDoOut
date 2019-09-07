using InDoOut_Console.Display;
using InDoOut_Console.ProgramView;
using InDoOut_Core.Entities.Programs;
using InDoOut_Core.Functions;
using InDoOut_Executable_Core.Location;
using InDoOut_Executable_Core.Storage;
using InDoOut_Json_Storage;
using InDoOut_Plugins.Loaders;
using System;
using System.Threading;

namespace InDoOut_Console
{
    class Start
    {
        static void Main(string[] args)
        {
            var originalBackgroundColour = Console.BackgroundColor;

            Console.BackgroundColor = ConsoleColor.DarkGray;
            Console.WriteLine();

            ColourConsole.WriteLine(new ColourBlock("in", ConsoleColor.Red), new ColourBlock(" > ", ConsoleColor.Gray), new ColourBlock("do", ConsoleColor.Red), new ColourBlock(" > ", ConsoleColor.Gray), new ColourBlock("out", ConsoleColor.Red));

            Console.WriteLine();
            Console.BackgroundColor = originalBackgroundColour;

            ColourConsole.WriteInfoLine("Loading plugins...");

            var pluginLoader = new PluginDirectoryLoader(new PluginLoader(), StandardLocations.Instance);
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

            if (args.Length > 0)
            {
                var programToStart = args[0];
                if (!string.IsNullOrEmpty(programToStart))
                {
                    ColourConsole.WriteInfoLine(new ColourBlock("Attempting to load program at "), new ColourBlock(programToStart, ConsoleColor.Yellow), new ColourBlock("..."));

                    var program = new Program();
                    var storage = new ProgramJsonStorer(new FunctionBuilder(), LoadedPlugins.Instance, programToStart);
                    var loadResult = storage.Load(program);

                    if (loadResult == LoadResult.OK)
                    {
                        ColourConsole.WriteInfoLine("Program loaded successfully.", ConsoleColor.Green);

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
                        ColourConsole.WriteErrorLine($"Program could not be loaded due to an error. ({loadResult})");
                    }
                }
                else
                {
                    ColourConsole.WriteErrorLine("Program to load is empty.");
                }
            }
            else
            {
                ColourConsole.WriteErrorLine("No program to load.");
            }

            Console.WriteLine();
            ColourConsole.WriteInfo("Press any key to close. ");

            _ = Console.ReadLine();
        }
    }
}
