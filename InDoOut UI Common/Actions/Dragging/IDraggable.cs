using InDoOut_UI_Common.InterfaceElements;
using System.Windows;

namespace InDoOut_UI_Common.Actions.Dragging
{
    public interface IDraggable
    {
        void DragStarted(IElementDisplay view);
        void DragMoved(IElementDisplay view, Point delta);
        void DragEnded(IElementDisplay view);

        bool CanDrag(IElementDisplay view);
    }
}
