namespace InDoOut_Console_Common.Commands
{
    public interface ICommand
    {
        bool Matches(string command);
        bool Trigger(string parameters);
    }
}
