using InDoOut_UI_Common.InterfaceElements;

namespace InDoOut_Display.UI.Controls.Screens
{
    public interface IScreenConnections : IProgramDisplay, IElementDisplay
    {
        IScreen CurrentScreen { get; }
    }
}
