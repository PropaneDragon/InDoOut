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
        public Size Size { get => new Size(ActualWidth, ActualHeight); set => UpdateSize(value); }

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
            var size = Size;
            var inBounds = point.X > -distance && point.X < (size.Width + distance) && point.Y > -distance && point.Y < (size.Height + distance);
            var nearLeft = PointWithin(point.X, Margin.Left - distance, Margin.Left + distance);
            var nearTop = PointWithin(point.Y, Margin.Top - distance, Margin.Top + distance);
            var nearRight = PointWithin(point.X, (Margin.Left + size.Width) - distance, (Margin.Left + size.Width) + distance);
            var nearBottom = PointWithin(point.Y, (Margin.Top + size.Height) - distance, (Margin.Top + size.Height) + distance);

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

        public void SetEdgeDistance(IScreen screen, ResizeEdge edge, double distance)
        {
            if (screen != null && edge.ValidEdge() && !edge.IsCorner())
            {
                var calculatedSize = CalculateSizeFromScreen(screen);
                var currentDistance = (edge == ResizeEdge.Left || edge == ResizeEdge.Right) ? calculatedSize.Width : calculatedSize.Height;
                var adjustment = currentDistance - distance;

                UpdateMarginForEdge(edge, adjustment);
            }
        }

        private Size CalculateSizeFromScreen(IScreen screen)
        {
            return new Size(screen.Size.Width - Margin.Left - Margin.Right, screen.Size.Height - Margin.Top - Margin.Bottom);
        }

        private void UpdateSize(Size size)
        {
            Width = size.Width;
            Height = size.Height;
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

        private double GetMarginForEdge(ResizeEdge edge)
        {
            return edge switch
            {
                ResizeEdge.Bottom => Margin.Bottom,
                ResizeEdge.Left => Margin.Left,
                ResizeEdge.Right => Margin.Right,
                ResizeEdge.Top => Margin.Top,

                _ => 0
            };
        }

        private void UpdateMarginForEdge(ResizeEdge edge, double margin)
        {
            var currentMargins = Margin;

            switch (edge)
            {
                case ResizeEdge.Bottom:
                    currentMargins.Bottom += margin;
                    break;
                case ResizeEdge.Left:
                    currentMargins.Left += margin;
                    break;
                case ResizeEdge.Right:
                    currentMargins.Right += margin;
                    break;
                case ResizeEdge.Top:
                    currentMargins.Top += margin;
                    break;
            }

            Margin = currentMargins;
        }
    }
}
