using InDoOut_Display_Core.Actions.Resizing;
using InDoOut_Display_Core.Actions.Scaling;
using InDoOut_UI_Common.Actions.Dragging;
using InDoOut_UI_Common.Actions.Selecting;
using System.Windows;

namespace InDoOut_Display_Core.Elements
{
    /// <summary>
    /// Represents a container for an element that has static margins, whereby the margins stay the same
    /// regardless of the size or scale of the container.
    /// </summary>
    public interface IStaticMarginElementContainer : IResizable, IScalable, ISelectable, IDraggable
    {
        /// <summary>
        /// The percentage from either side of the parent container that this
        /// container should be.
        /// </summary>
        Thickness MarginPercentages { get; set; }
    }
}
