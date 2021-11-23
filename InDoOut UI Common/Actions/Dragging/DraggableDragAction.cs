using InDoOut_UI_Common.InterfaceElements;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace InDoOut_UI_Common.Actions.Dragging
{
    public class DraggableDragAction : DragAction
    {
        private readonly Dictionary<IDraggable, Point> _initialPositions = new();
        private readonly ICommonDisplay _display = null;

        public DraggableDragAction(ICommonDisplay display, IEnumerable<IDraggable> draggables, Point mousePosition)
        {
            _ = base.MouseLeftDown(mousePosition);

            if (display != null && draggables != null && draggables.All(draggable => draggable.CanDrag(display)) && draggables.All(draggable => draggable is FrameworkElement))
            {
                _display = display;
                _initialPositions.Clear();

                foreach (var draggable in draggables)
                {
                    _initialPositions[draggable] = _display.GetPosition(draggable as FrameworkElement);

                    draggable.DragStarted(_display);
                }
            }
            else
            {
                Abort();
            }
        }

        public override bool MouseLeftMove(Point mousePosition)
        {
            _ = base.MouseLeftMove(mousePosition);

            if (_display != null && _initialPositions.Keys.All(draggable => draggable is FrameworkElement))
            {
                foreach (var elementPosition in _initialPositions)
                {
                    _display.SetPosition(elementPosition.Key as FrameworkElement, new Point(elementPosition.Value.X - MouseDelta.X, elementPosition.Value.Y - MouseDelta.Y));

                    elementPosition.Key.DragMoved(_display, MouseDelta);
                }

                return true;
            }

            Abort();
            return false;
        }

        public override bool MouseLeftUp(Point mousePosition)
        {
            _ = base.MouseLeftUp(mousePosition);

            foreach (var draggable in _initialPositions)
            {
                draggable.Key.DragEnded(_display);
            }

            Finish(null);
            return true;
        }
    }
}
