namespace InDoOut_Display.UI.Controls.Screens
{
    public interface IScreenConnections : IElementHost, IFunctionHost
    {
        IScreen CurrentScreen { get; }
    }
}
