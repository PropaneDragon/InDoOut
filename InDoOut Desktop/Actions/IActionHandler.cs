namespace InDoOut_Desktop.Actions
{
    internal interface IActionHandler : IAction
    {
        IAction DefaultAction { get; }
        IAction CurrentAction { get; }
    }
}
