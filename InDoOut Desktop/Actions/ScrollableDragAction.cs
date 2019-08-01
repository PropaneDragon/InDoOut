using InDoOut_Desktop.UI.Interfaces;
using System.Windows;

namespace InDoOut_Desktop.Actions
{
    internal class ScrollableDragAction : DragAction
    {
        private Point _initialScrollPosition = new Point();

        public IScrollable Scrollable { get; set; } = null;

        public ScrollableDragAction(IScrollable scrollable, Point mousePosition)
        {
            base.MouseLeftDown(mousePosition);

            Scrollable = scrollable;

            if (Scrollable != null && Scrollable is FrameworkElement element)
            {
                _initialScrollPosition = Scrollable.Offset;
            }
            else
            {
                Abort();
            }
        }

        public override bool MouseLeftMove(Point mousePosition)
        {
            base.MouseLeftMove(mousePosition);

            if (Scrollable != null && Scrollable is FrameworkElement element)
            {
                Scrollable.Offset = new Point(_initialScrollPosition.X + MouseDelta.X, _initialScrollPosition.Y + MouseDelta.Y);

                return true;
            }

            Abort();

            return false;
        }

        public override bool MouseLeftUp(Point mousePosition)
        {
            base.MouseLeftUp(mousePosition);

            if (Scrollable != null && Scrollable is FrameworkElement element)
            {
                Finish(null);

                return true;
            }

            Abort();

            return false;
        }
    }
}
