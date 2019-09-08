using InDoOut_Desktop.UI.Interfaces;

namespace InDoOut_Desktop.Actions.Dragging
{
    public interface IDraggable
    {
        void DragStarted(IBlockView view);
        void DragMoved(IBlockView view);
        void DragEnded(IBlockView view);

        bool CanDrag(IBlockView view);
    }
}
