namespace InDoOut_UI_Common.Actions
{
    public interface IActionHandler : IAction
    {
        IAction DefaultAction { get; }
        IAction CurrentAction { get; }
    }
}
