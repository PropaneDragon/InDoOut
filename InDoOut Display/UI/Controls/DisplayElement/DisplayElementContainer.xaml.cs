using InDoOut_Display.Actions.Resizing;
using InDoOut_Display.UI.Controls.Screens;
using InDoOut_Display_Core.Elements;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace InDoOut_Display.UI.Controls.DisplayElement
{
    public partial class DisplayElementContainer : UserControl, IResizable
    {
        public IDisplayElement AssociatedDisplayElement { get => ContentPresenter_Element.Content as IDisplayElement; set => SetDisplayElement(value); }
        public Size Size => new Size(ContentPresenter_Element.ActualWidth, ContentPresenter_Element.ActualHeight);

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
            var localPoint = TranslatePoint(point, ContentPresenter_Element);
            var inBounds = localPoint.X > -distance && localPoint.X < (size.Width + distance) && localPoint.Y > -distance && localPoint.Y < (size.Height + distance);
            var nearLeft = PointWithin(localPoint.X, -distance, distance);
            var nearTop = PointWithin(localPoint.Y, -distance, distance);
            var nearRight = PointWithin(localPoint.X, size.Width - distance, size.Width + distance);
            var nearBottom = PointWithin(localPoint.Y, size.Height - distance, size.Height + distance);

            if (inBounds)
            {
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
            }

            return ResizeEdge.None;
        }

        public void SetEdgeToMouse(IScreen screen, ResizeEdge edge)
        {
            if (screen != null && edge.ValidEdge())
            {
                var mousePercentage = GetMouseLocationAsPercentage();
                
                if (edge == ResizeEdge.Left || edge == ResizeEdge.BottomLeft || edge == ResizeEdge.TopLeft)
                {
                    Column_Width_Left.Width = new GridLength(mousePercentage.X, GridUnitType.Star);
                }
                
                if (edge == ResizeEdge.Right || edge == ResizeEdge.BottomRight || edge == ResizeEdge.TopRight)
                {
                    Column_Width_Right.Width = new GridLength(1d - mousePercentage.X, GridUnitType.Star);
                }

                if (edge == ResizeEdge.Top || edge == ResizeEdge.TopLeft || edge == ResizeEdge.TopRight)
                {
                    Row_Height_Above.Height = new GridLength(mousePercentage.Y, GridUnitType.Star);
                }

                if (edge == ResizeEdge.Bottom || edge == ResizeEdge.BottomLeft || edge == ResizeEdge.BottomRight)
                {
                    Row_Height_Below.Height = new GridLength(1d - mousePercentage.Y, GridUnitType.Star);
                }

                UpdateElementPercentages();
            }
        }

        private void UpdateElementPercentages()
        {
            Column_Width_Element.Width = new GridLength(1d - (Column_Width_Left.Width.Value + Column_Width_Right.Width.Value), GridUnitType.Star);
            Row_Height_Element.Height = new GridLength(1d - (Row_Height_Above.Height.Value + Row_Height_Below.Height.Value), GridUnitType.Star);
        }

        private Point GetMouseLocationAsPercentage()
        {
            var position = Mouse.GetPosition(this);
            var overallSize = new Size(ActualWidth, ActualHeight);

            return new Point(position.X / overallSize.Width, position.Y / overallSize.Height);
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
