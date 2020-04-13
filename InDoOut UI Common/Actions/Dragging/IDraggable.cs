using InDoOut_UI_Common.InterfaceElements;
using System.Windows;

namespace InDoOut_UI_Common.Actions.Dragging
{
    public interface IDraggable
    {
        void DragStarted(ICommonDisplay display);
        void DragMoved(ICommonDisplay display, Point delta);
        void DragEnded(ICommonDisplay display);

        bool CanDrag(ICommonDisplay display);
    }
}
