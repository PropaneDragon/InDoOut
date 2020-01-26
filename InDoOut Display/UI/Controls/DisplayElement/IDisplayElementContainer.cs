using InDoOut_Display.Actions.Resizing;
using InDoOut_Display.Actions.Scaling;
using InDoOut_UI_Common.Actions.Dragging;
using InDoOut_UI_Common.Actions.Selecting;
using InDoOut_UI_Common.InterfaceElements;

namespace InDoOut_Display.UI.Controls.DisplayElement
{
    public interface IDisplayElementContainer : IResizable, IScalable, ISelectable, IDraggable
    {
        ProgramViewMode ViewMode { get; set; }
    }
}
