using InDoOut_Display.Actions.Resizing;
using InDoOut_Display.UI.Controls.Screens;
using InDoOut_Display_Core.Elements;
using System.Windows;
using System.Windows.Controls;

namespace InDoOut_Display.UI.Controls.DisplayElement
{
    public partial class DisplayElementContainer : UserControl, IResizable
    {
        public IDisplayElement AssociatedDisplayElement { get => ContentPresenter_Element.Content as IDisplayElement; set => SetDisplayElement(value); }

        public DisplayElementContainer(IDisplayElement element = null)
        {
            InitializeComponent();

            AssociatedDisplayElement = element;
        }

        public void ResizeEnded(IScreen screen)
        {
        }

        public void ResizeStarted(IScreen screen)
        {
        }

        public bool CanResize(IScreen screen)
        {
            return true;
        }

        public bool CloseToEdge(IScreen screen, Point point, double distance = 5)
        {
            return GetCloseEdge(screen, point, distance) != ResizeEdge.None;
        }

        public ResizeEdge GetCloseEdge(IScreen screen, Point point, double distance = 5)
        {
            var size = new Size(ActualWidth, ActualHeight);
            var inBounds = point.X > -distance && point.X < (size.Width + distance) && point.Y > -distance && point.Y < (size.Height + distance);
            var nearLeft = inBounds && PointWithin(point.X, -distance, distance);
            var nearTop = inBounds && PointWithin(point.Y, -distance, distance);
            var nearRight = inBounds && PointWithin(point.X, size.Width - distance, size.Width + distance);
            var nearBottom = inBounds && PointWithin(point.Y, size.Height - distance, size.Height + distance);

            if (nearLeft)
            {
                return nearTop ? ResizeEdge.TopLeft : nearBottom ? ResizeEdge.BottomLeft : ResizeEdge.Left;
            }
            else if (nearRight)
            {
                return nearTop ? ResizeEdge.TopRight : nearBottom ? ResizeEdge.BottomRight : ResizeEdge.Right;
            }
            else if (nearBottom)
            {
                return ResizeEdge.Bottom;
            }
            else if (nearTop)
            {
                return ResizeEdge.Top;
            }

            return ResizeEdge.None;
        }

        private bool PointWithin(double point, double min, double max)
        {
            return point > min && point < max;
        }

        private void SetDisplayElement(IDisplayElement element)
        {
            if (element != null && element is UIElement uiElement)
            {
                ContentPresenter_Element.Content = uiElement;
            }
        }
    }
}
