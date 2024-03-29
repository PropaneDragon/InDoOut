﻿using InDoOut_UI_Common.InterfaceElements;
using System.Windows;

namespace InDoOut_UI_Common.Actions.Dragging
{
    public class ScrollableDragAction : DragAction
    {
        private Point _initialScrollPosition = new();

        public IScrollable Scrollable { get; set; } = null;

        public ScrollableDragAction(IScrollable scrollable, Point mousePosition)
        {
            _ = base.MouseLeftDown(mousePosition);

            Scrollable = scrollable;

            if (Scrollable != null && Scrollable is FrameworkElement)
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
            _ = base.MouseLeftMove(mousePosition);

            if (Scrollable != null && Scrollable is FrameworkElement)
            {
                Scrollable.Offset = new Point(_initialScrollPosition.X + MouseDelta.X, _initialScrollPosition.Y + MouseDelta.Y);

                return true;
            }

            Abort();

            return false;
        }

        public override bool MouseLeftUp(Point mousePosition)
        {
            _ = base.MouseLeftUp(mousePosition);

            if (Scrollable != null && Scrollable is FrameworkElement)
            {
                Finish(null);

                return true;
            }

            Abort();

            return false;
        }
    }
}
