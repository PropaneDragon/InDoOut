using InDoOut_Desktop.UI.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace InDoOut_Desktop.Actions.Dragging
{
    internal class DraggableDragAction : InDoOut_UI_Common.Actions.Dragging.DragAction
    {
        private readonly Dictionary<IDraggable, Point> _initialPositions = new Dictionary<IDraggable, Point>();
        private readonly IBlockView _blockView = null;

        public DraggableDragAction(IBlockView blockView, IEnumerable<IDraggable> draggables, Point mousePosition)
        {
            _ = base.MouseLeftDown(mousePosition);

            if (blockView != null && draggables != null && draggables.All(draggable => draggable.CanDrag(blockView)) && draggables.All(draggable => draggable is FrameworkElement))
            {
                _blockView = blockView;
                _initialPositions.Clear();

                foreach (var draggable in draggables)
                {
                    _initialPositions[draggable] = _blockView.GetPosition(draggable as FrameworkElement);

                    draggable.DragStarted(_blockView);
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

            if (_blockView != null && _initialPositions.Keys.All(draggable => draggable is FrameworkElement))
            {
                foreach (var elementPosition in _initialPositions)
                {
                    _blockView.SetPosition(elementPosition.Key as FrameworkElement, new Point(elementPosition.Value.X - MouseDelta.X, elementPosition.Value.Y - MouseDelta.Y));

                    elementPosition.Key.DragMoved(_blockView);
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
                draggable.Key.DragEnded(_blockView);
            }

            Finish(null);
            return true;
        }
    }
}
