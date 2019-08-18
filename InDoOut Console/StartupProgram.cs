using InDoOut_Core.Entities.Programs;
using System;

namespace InDoOut_Console
{
    class StartupProgram
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var loadedProgram = new Program();

            Console.WriteLine(loadedProgram.Id);
            _ = Console.ReadLine();
        }
    }
}
