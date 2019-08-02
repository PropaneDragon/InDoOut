using InDoOut_Desktop.UI.Interfaces;
using System.Windows;

namespace InDoOut_Desktop.Actions
{
    internal class DraggableDragAction : DragAction
    {
        private Point _initialControlPosition = new Point();
        private IDraggable _draggable = null;
        private IBlockView _blockView = null;

        public DraggableDragAction(IBlockView blockView, IDraggable draggable, Point mousePosition)
        {
            base.MouseLeftDown(mousePosition);

            if (blockView != null && draggable != null && draggable.CanDrag() && draggable is FrameworkElement element)
            {
                _blockView = blockView;

                _draggable = draggable;
                _draggable.DragStarted(_blockView);

                _initialControlPosition = _blockView.GetPosition(element);
            }
            else
            {
                Abort();
            }
        }

        public override bool MouseLeftMove(Point mousePosition)
        {
            base.MouseLeftMove(mousePosition);

            if (_draggable != null && _blockView != null && _draggable is FrameworkElement element)
            {
                _blockView.SetPosition(element, new Point(_initialControlPosition.X - MouseDelta.X, _initialControlPosition.Y - MouseDelta.Y));

                _draggable.DragMoved(_blockView);

                return true;
            }

            Abort();
            return false;
        }

        public override bool MouseLeftUp(Point mousePosition)
        {
            base.MouseLeftUp(mousePosition);

            if (_draggable != null)
            {
                _draggable.DragEnded(_blockView);

                Finish(null);
                return true;
            }

            Abort();
            return false;
        }
    }
}
