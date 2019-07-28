using System.Windows;
using System.Windows.Controls;

namespace InDoOut_Desktop.Actions
{
    internal class ScrollViewDragAction : DragAction
    {
        private Point _initialScrollPosition = new Point();

        public ScrollViewer ScrollViewer { get; set; } = null;

        public ScrollViewDragAction(ScrollViewer scrollViewer, Point mousePosition)
        {
            base.MouseLeftDown(mousePosition);

            ScrollViewer = scrollViewer;

            if (ScrollViewer != null)
            {
                _initialScrollPosition = new Point(ScrollViewer.HorizontalOffset, ScrollViewer.VerticalOffset);

                ScrollViewer.CaptureMouse();
            }
        }

        public override bool MouseLeftMove(Point mousePosition)
        {
            base.MouseLeftMove(mousePosition);

            if (ScrollViewer != null && ScrollViewer.IsMouseCaptured)
            {
                ScrollViewer.ScrollToHorizontalOffset(_initialScrollPosition.X + MouseDelta.X);
                ScrollViewer.ScrollToVerticalOffset(_initialScrollPosition.Y + MouseDelta.Y);

                return true;
            }

            Abort();

            return false;
        }

        public override bool MouseLeftUp(Point mousePosition)
        {
            base.MouseLeftUp(mousePosition);

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
