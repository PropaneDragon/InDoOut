namespace InDoOut_Console_Common.Commands
{
    public abstract class Command : ICommand
    {
        public abstract bool Matches(string command);
        public abstract bool Trigger(string[] parameters);
    }
}
