using InDoOut_Display_Core.Elements;
using System.Windows;
using System.Windows.Controls;

namespace InDoOut_Display.UI.Controls.Screens
{
    public partial class ScreenItem : UserControl, IScreenItem
    {
        public ScreenItem()
        {
            InitializeComponent();
        }

        public bool AddDisplayElement(IDisplayElement displayElement)
        {
            if (displayElement != null && displayElement is UIElement uiElement)
            {
                _ = Grid_Elements.Children.Add(uiElement);
            }

            return false;
        }

        public bool RemoveDisplayElement(IDisplayElement displayElement)
        {
            if (displayElement != null)
            {

            }

            return false;
        }

        public ScreenItemEdge GetCloseEdge(Point point, double distance = 5d)
        {
            var size = new Size(ActualWidth, ActualHeight);
            var inBounds = point.X > -distance && point.X < (size.Width + distance) && point.Y > -distance && point.Y < (size.Height + distance);
            var nearLeft = inBounds && PointWithin(point.X, -distance, distance);
            var nearTop = inBounds && PointWithin(point.Y, -distance, distance);
            var nearRight = inBounds && PointWithin(point.X, size.Width - distance, size.Width + distance);
            var nearBottom = inBounds && PointWithin(point.Y, size.Height - distance, size.Height + distance);

            if (nearLeft)
            {
                return nearTop ? ScreenItemEdge.TopLeft : nearBottom ? ScreenItemEdge.BottomLeft : ScreenItemEdge.Left;
            }
            else if (nearRight)
            {
                return nearTop ? ScreenItemEdge.TopRight : nearBottom ? ScreenItemEdge.BottomRight : ScreenItemEdge.Right;
            }
            else if (nearBottom)
            {
                return ScreenItemEdge.Bottom;
            }
            else if (nearTop)
            {
                return ScreenItemEdge.Top;
            }

            return ScreenItemEdge.None;
        }

        public bool PointCloseToScreenItemEdge(Point point, double distance = 5d)
        {
            return GetCloseEdge(point, distance) != ScreenItemEdge.None;
        }

        private bool PointWithin(double point, double min, double max)
        {
            return point > min && point < max;
        }
    }
}
