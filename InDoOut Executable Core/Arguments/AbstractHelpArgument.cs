using System;

namespace InDoOut_Executable_Core.Arguments
{
    public abstract class AbstractHelpArgument : Argument
    {
        public AbstractHelpArgument() : base("?", "Shows all available help.", false)
        {
        }

        public override void Trigger(IArgumentHandler handler)
        {
            ShowHelp(handler);
        }

        protected abstract void ShowHelp(IArgumentHandler handler);
    }
}
