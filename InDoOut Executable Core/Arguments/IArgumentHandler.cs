using InDoOut_Core.Instancing;
using System.Collections.Generic;

namespace InDoOut_Executable_Core.Arguments
{
    public interface IArgumentHandler : ISingleton<IArgumentHandler>
    {
        char[] ArgumentKeyPrefixes { get; set; }
        char[] KeyValueSeparators { get; set; }

        IReadOnlyList<IArgument> Arguments { get; }

        bool AddArgument(IArgument argument);
        bool AddArguments(params IArgument[] arguments);
        bool AddArguments(IEnumerable<IArgument> arguments);

        void Process(params string[] rawArguments);

        string FormatKeyValue(IArgument argument);
        string FormatDescription(IArgument argument);
    }
}
