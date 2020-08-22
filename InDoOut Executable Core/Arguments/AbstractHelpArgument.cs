namespace InDoOut_Executable_Core.Arguments
{
    public abstract class AbstractHelpArgument : Argument
    {
        public bool ShouldShowHelp { get; private set; } = false;
        public bool ShowHelpWhenTriggered = true;

        public AbstractHelpArgument(bool showHelpWhenTriggered = true) : base("?", "Shows this help section.", false)
        {
            ShowHelpWhenTriggered = showHelpWhenTriggered;
        }

        public override void Trigger(IArgumentHandler handler)
        {
            ShouldShowHelp = true;

            if (ShowHelpWhenTriggered)
            {
                ShowHelp(handler);
            }
        }

        public abstract void ShowHelp(IArgumentHandler handler);
    }
}
