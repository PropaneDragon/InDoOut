namespace InDoOut_Console_Common.Commands
{
    public class ExitCommand : SimpleCommand
    {
        private readonly CommandHandler _commandHandler = null;

        public ExitCommand(CommandHandler handler) : base("exit", "quit", "close")
        {
            _commandHandler = handler;
        }

        public override bool Trigger(string[] parameters)
        {
            if (_commandHandler != null)
            {
                _commandHandler.Exit = true;

                return true;
            }

            return false;
        }
    }
}
