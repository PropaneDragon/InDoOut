﻿using System.Windows;
using System.Windows.Controls;

namespace InDoOut_Desktop.Actions
{
    internal class ElementDragAction : DragAction
    {
        private Point _initialControlPosition = new Point();
        private UIElement _element = null;

        public ElementDragAction(UIElement _, Point _1)
        {
            /*base.MouseLeftDown(mousePosition);

            if (element != null && FindParent<Canvas>(element) != null)
            {
                _element = element;
                _initialControlPosition = new Point(Canvas.GetLeft(_element), Canvas.GetTop(_element));

                element.CaptureMouse();
            }
            else
            {
                Abort();
            }*/
        }

        public override bool MouseLeftMove(Point mousePosition)
        {
            _ = base.MouseLeftMove(mousePosition);

            if (_element != null && _element.IsMouseCaptured)
            {
                Canvas.SetLeft(_element, _initialControlPosition.X - MouseDelta.X);
                Canvas.SetTop(_element, _initialControlPosition.Y - MouseDelta.Y);

                return true;
            }

            Abort();
            return false;
        }

        public override bool MouseLeftUp(Point mousePosition)
        {
            _ = base.MouseLeftUp(mousePosition);

            if (_element != null)
            {
                _element.ReleaseMouseCapture();

                Finish(null);
                return true;
            }

            Abort();
            return false;
        }
    }
}
