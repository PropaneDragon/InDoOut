using InDoOut_Console.Display;
using InDoOut_Core.Entities.Programs;
using System;
using System.Linq;
using System.Threading;

namespace InDoOut_Console.ProgramView
{
    internal class ProgramDisplay
    {
        private readonly IProgram _program = null;

        public ProgramDisplay(IProgram program)
        {
            _program = program;
        }

        public void ShowRunStatus()
        {
            if (_program != null)
            {
                var originalCursorPositionLeft = Console.CursorLeft;
                var originalCursorPositionTop = Console.CursorTop;
                var currentCursorPositionTop = Console.CursorTop;
                var charactersToWrite = 0;

                while (IsProgramRunning())
                {
                    currentCursorPositionTop = Console.CursorTop;
                    charactersToWrite = Console.BufferWidth * ((currentCursorPositionTop - originalCursorPositionTop) + 1);

                    Console.SetCursorPosition(originalCursorPositionLeft, originalCursorPositionTop);
                    Console.Write(new string(' ', charactersToWrite));
                    Console.SetCursorPosition(originalCursorPositionLeft, originalCursorPositionTop);

                    var runningFunctions = _program.Functions.Where(function => function.Running);

                    Console.WriteLine();
                    ColourConsole.WriteInfoLine("Running:");

                    foreach (var runningFunction in runningFunctions)
                    {
                        ColourConsole.WriteLine(new ColourBlock($"{runningFunction.SafeName} ", ConsoleColor.Yellow), new ColourBlock($"[{runningFunction.Id}] ", ConsoleColor.DarkYellow), new ColourBlock($"({runningFunction.State})", ConsoleColor.DarkRed));
                        ColourConsole.WriteLine("Properties:", ConsoleColor.Gray);

                        foreach (var property in runningFunction.Properties)
                        {
                            ColourConsole.Write(new ColourBlock($"{property.Name}: ", ConsoleColor.Cyan), new ColourBlock($"{property.RawValue}     ", ConsoleColor.White));
                        }

                        Console.WriteLine();
                        Console.WriteLine();
                    }

                    Thread.Sleep(TimeSpan.FromMilliseconds(500));
                }

                currentCursorPositionTop = Console.CursorTop;
                charactersToWrite = Console.BufferWidth * ((currentCursorPositionTop - originalCursorPositionTop) + 1);

                Console.SetCursorPosition(originalCursorPositionLeft, originalCursorPositionTop);
                Console.Write(new string(' ', charactersToWrite));
                Console.SetCursorPosition(originalCursorPositionLeft, originalCursorPositionTop);
            }
        }

        private bool IsProgramRunning()
        {
            if (_program != null)
            {
                var maxRetry = 3;

                for (var retryCount = 0; retryCount < maxRetry; ++retryCount)
                {
                    if (_program.Running)
                    {
                        return true;
                    }
                    else
                    {
                        Thread.Sleep(TimeSpan.FromMilliseconds(10));
                    }
                }
            }

            return false;
        }
    }
}
