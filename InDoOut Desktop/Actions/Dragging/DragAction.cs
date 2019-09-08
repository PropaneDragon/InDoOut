using System.Windows;

namespace InDoOut_Desktop.Actions.Dragging
{
    internal abstract class DragAction : Action
    {
        private bool _dragging = false;
        private Point _lastMousePosition = new Point();

        internal Point MouseDelta { get; private set; } = new Point();

        public override bool MouseLeftDown(Point mousePosition)
        {
            _lastMousePosition = mousePosition;
            _dragging = true;

            return base.MouseLeftDown(mousePosition);
        }

        public override bool MouseLeftMove(Point mousePosition)
        {
            if (_dragging)
            {
                var currentMousePosition = mousePosition;

                MouseDelta = new Point(_lastMousePosition.X - currentMousePosition.X, _lastMousePosition.Y - currentMousePosition.Y);
            }

            return base.MouseLeftMove(mousePosition);
        }

        public override bool MouseLeftUp(Point mousePosition)
        {
            _dragging = false;

            return base.MouseLeftUp(mousePosition);
        }
    }
}
