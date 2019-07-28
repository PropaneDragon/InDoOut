using System.Windows;
using System.Windows.Controls;

namespace InDoOut_Desktop.Actions
{
    internal class ScrollViewDragAction : Action
    {
        private Point _lastMousePosition = new Point();
        private Point _lastScrollPosition = new Point();

        public ScrollViewer ScrollViewer { get; set; } = null;

        public ScrollViewDragAction(ScrollViewer scrollViewer, Point mousePosition)
        {
            ScrollViewer = scrollViewer;

            if (ScrollViewer != null)
            {
                _lastMousePosition = mousePosition;
                _lastScrollPosition = new Point(ScrollViewer.HorizontalOffset, ScrollViewer.VerticalOffset);

                ScrollViewer.CaptureMouse();
            }
        }

        public override bool MouseLeftMove(Point mousePosition)
        {
            if (ScrollViewer != null && ScrollViewer.IsMouseCaptured)
            {
                var currentMousePosition = mousePosition;

                ScrollViewer.ScrollToHorizontalOffset(_lastScrollPosition.X + (_lastMousePosition.X - currentMousePosition.X));
                ScrollViewer.ScrollToVerticalOffset(_lastScrollPosition.Y + (_lastMousePosition.Y - currentMousePosition.Y));

                return true;
            }

            Abort();

            return false;
        }

        public override bool MouseLeftUp(Point mousePosition)
        {
            if (ScrollViewer != null)
            {
                ScrollViewer.ReleaseMouseCapture();

                Finish(null);

                return true;
            }

            Abort();

            return false;
        }
    }
}
