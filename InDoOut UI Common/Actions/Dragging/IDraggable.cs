namespace InDoOut_UI_Common.Actions.Dragging
{
    public interface IDraggable<ViewType>
    {
        void DragStarted(ViewType view);
        void DragMoved(ViewType view);
        void DragEnded(ViewType view);

        bool CanDrag(ViewType view);
    }
}
