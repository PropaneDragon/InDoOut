namespace InDoOut_Console_Common.Commands
{
    public interface ISimpleCommand : ICommand
    {
        public string[] Aliases { get; }
    }
}
