using System;
using System.Linq;

namespace InDoOut_Console_Common.Commands
{
    public abstract class SimpleCommand : Command, ISimpleCommand
    {
        public virtual string[] Aliases { get; private set; } = Array.Empty<string>();

        private SimpleCommand()
        {
        }

        public SimpleCommand(params string[] aliases) : this()
        {
            Aliases = aliases;
        }

        public override bool Matches(string command) => Aliases.Contains(command);
    }
}
