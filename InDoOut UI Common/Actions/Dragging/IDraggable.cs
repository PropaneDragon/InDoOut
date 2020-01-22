using InDoOut_UI_Common.InterfaceElements;

namespace InDoOut_UI_Common.Actions.Dragging
{
    public interface IDraggable
    {
        void DragStarted(IElementDisplay view);
        void DragMoved(IElementDisplay view);
        void DragEnded(IElementDisplay view);

        bool CanDrag(IElementDisplay view);
    }
}
