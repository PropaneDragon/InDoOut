using InDoOut_Desktop.UI.Interfaces;
using System.Windows;
using System.Windows.Controls;

namespace InDoOut_Desktop.Actions
{
    internal class DraggableDragAction : DragAction
    {
        private Point _initialControlPosition = new Point();
        private IDraggable _draggable = null;
        private IElementDisplay _elementDisplay = null;

        public DraggableDragAction(IElementDisplay elementDisplay, IDraggable draggable, Point mousePosition)
        {
            base.MouseLeftDown(mousePosition);

            if (elementDisplay != null && draggable != null && draggable.CanDrag() && draggable is FrameworkElement element)
            {
                _elementDisplay = elementDisplay;

                _draggable = draggable;
                _draggable.DragStarted();

                _initialControlPosition = _elementDisplay.GetPosition(element);
            }
            else
            {
                Abort();
            }
        }

        public override bool MouseLeftMove(Point mousePosition)
        {
            base.MouseLeftMove(mousePosition);

            if (_draggable != null && _elementDisplay != null && _draggable is FrameworkElement element)
            {
                _elementDisplay.SetPosition(element, new Point(_initialControlPosition.X - MouseDelta.X, _initialControlPosition.Y - MouseDelta.Y));

                _draggable.DragMoved();

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
                _draggable.DragEnded();

                Finish(null);
                return true;
            }

            Abort();
            return false;
        }
    }
}
