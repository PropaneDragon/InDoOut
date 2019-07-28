using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace InDoOut_Desktop.Actions
{
    internal class ElementDragAction : DragAction
    {
        private Point _initialControlPosition = new Point();
        private UIElement _element = null;

        public ElementDragAction(UIElement element, Point mousePosition)
        {
            base.MouseLeftDown(mousePosition);

            if (element != null && FindParent<Canvas>(element) != null)
            {
                _element = element;
                _initialControlPosition = new Point(Canvas.GetLeft(_element), Canvas.GetTop(_element));

                element.CaptureMouse();
            }
            else
            {
                Abort();
            }
        }

        public override bool MouseLeftMove(Point mousePosition)
        {
            base.MouseLeftMove(mousePosition);

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
            base.MouseLeftUp(mousePosition);

            if (_element != null)
            {
                _element.ReleaseMouseCapture();

                Finish(null);
                return true;
            }

            Abort();
            return false;
        }

        protected T FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            if (child != null)
            {
                var parent = VisualTreeHelper.GetParent(child);

                if (parent == null)
                {
                    return null;
                }
                else if (parent is T control)
                {
                    return control;
                }

                return FindParent<T>(parent);
            }

            return default;
        }
    }
}
