using InDoOut_UI_Common.InterfaceElements;

namespace InDoOut_Display.UI.Controls.DisplayElement
{
    public interface IDisplayElementContainer : IStaticMarginElementContainer
    {
        ProgramViewMode ViewMode { get; set; }
    }
}
