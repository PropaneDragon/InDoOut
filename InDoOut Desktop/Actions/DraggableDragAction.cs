using System.Windows;
using System.Windows.Controls;

namespace InDoOut_Desktop.Actions
{
    internal class DraggableDragAction : DragAction
    {
        private Point _initialControlPosition = new Point();
        private UIElement _element = null;
        private IDraggable _draggable = null;

        public DraggableDragAction(IDraggable draggable, Point mousePosition)
        {
            base.MouseLeftDown(mousePosition);

            if (draggable != null && draggable.CanDrag() && draggable is UIElement element && FindParent<Canvas>(element) != null)
            {
                _draggable = draggable;
                _draggable.DragStarted();

                _element = element;
                _element.CaptureMouse();

                _initialControlPosition = new Point(Canvas.GetLeft(_element), Canvas.GetTop(_element));
            }
            else
            {
                Abort();
            }
        }

        public override bool MouseLeftMove(Point mousePosition)
        {
            base.MouseLeftMove(mousePosition);

            if (_draggable != null && _element != null && _element.IsMouseCaptured)
            {
                Canvas.SetLeft(_element, _initialControlPosition.X - MouseDelta.X);
                Canvas.SetTop(_element, _initialControlPosition.Y - MouseDelta.Y);

                _draggable.DragMoved();

                return true;
            }

            Abort();
            return false;
        }

        public override bool MouseLeftUp(Point mousePosition)
        {
            base.MouseLeftUp(mousePosition);

            if (_draggable != null && _element != null)
            {
                _element.ReleaseMouseCapture();
                _draggable.DragEnded();

                Finish(null);
                return true;
            }

            Abort();
            return false;
        }
    }
}
