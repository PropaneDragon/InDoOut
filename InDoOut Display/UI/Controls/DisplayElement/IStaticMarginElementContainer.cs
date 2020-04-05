using InDoOut_Display.Actions.Resizing;
using InDoOut_Display.Actions.Scaling;
using InDoOut_UI_Common.Actions.Dragging;
using InDoOut_UI_Common.Actions.Selecting;
using System.Windows;

namespace InDoOut_Display.UI.Controls.DisplayElement
{
    public interface IStaticMarginElementContainer : IResizable, IScalable, ISelectable, IDraggable
    {
        Thickness MarginPercentages { get; set; }
    }
}
