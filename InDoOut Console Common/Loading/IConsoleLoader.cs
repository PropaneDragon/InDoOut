using System;

namespace InDoOut_Console_Common.Loading
{
    public interface IConsoleLoader
    {
        string Name { get; }

        bool Load();
    }
}
