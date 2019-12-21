using InDoOut_Display.Actions.Resizing;
using InDoOut_Display.Actions.Scaling;
using InDoOut_Display.UI.Controls.Screens;
using InDoOut_Display_Core.Elements;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace InDoOut_Display.UI.Controls.DisplayElement
{
    public partial class DisplayElementContainer : UserControl, IResizable, IScalable
    {
        private static readonly Thickness SELECTED_BORDER_THICKNESS = new Thickness(2);
        private Thickness _borderThickness = new Thickness(0);
        private Thickness _marginThickness = new Thickness(0);

        public bool AutoScale { get; set; } = false;
        public double Scale { get; set; } = 1d;
        public IDisplayElement AssociatedDisplayElement { get => ContentPresenter_Element.Content as IDisplayElement; set => SetDisplayElement(value); }
        public Size Size => new Size(ContentPresenter_Element.ActualWidth, ContentPresenter_Element.ActualHeight);
        public Thickness MarginPercentages { get => GetMarginPercentages(); set => SetMarginPercentages(value); }

        public DisplayElementContainer(IDisplayElement element = null)
        {
            InitializeComponent();

            AssociatedDisplayElement = element;
        }

        public bool CanResize(IScreen screen) => true;

        public bool CanScale(IScreen screen) => true;

        public void ScaleChanged(IScreen screen)
        {
        }

        public void ResizeStarted(IScreen screen)
        {
            _borderThickness = Border_Presenter.BorderThickness;
            _marginThickness = Border_Presenter.Margin;

            Border_Presenter.BorderThickness = SELECTED_BORDER_THICKNESS;
            Border_Presenter.Margin = NegateThickness(SELECTED_BORDER_THICKNESS);
        }

        public void ResizeEnded(IScreen screen)
        {
            Border_Presenter.BorderThickness = _borderThickness;
            Border_Presenter.Margin = _marginThickness;
        }

        public bool CloseToEdge(IScreen screen, Point point, double distance = 5) => GetCloseEdge(screen, point, distance) != ResizeEdge.None;

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
                var margins = MarginPercentages;
                var minimumSize = 0.01;

                if (edge == ResizeEdge.Left || edge == ResizeEdge.BottomLeft || edge == ResizeEdge.TopLeft)
                {
                    margins.Left = minimumSize + mousePercentage.X + margins.Right < 1d ? mousePercentage.X : margins.Left;
                }
                
                if (edge == ResizeEdge.Right || edge == ResizeEdge.BottomRight || edge == ResizeEdge.TopRight)
                {
                    var adjustedMargin = 1d - mousePercentage.X;
                    margins.Right = minimumSize + adjustedMargin + margins.Left < 1d ? adjustedMargin : margins.Right;
                }

                if (edge == ResizeEdge.Top || edge == ResizeEdge.TopLeft || edge == ResizeEdge.TopRight)
                {
                    margins.Top = minimumSize + mousePercentage.Y + margins.Bottom < 1d ? mousePercentage.Y : margins.Top;
                }

                if (edge == ResizeEdge.Bottom || edge == ResizeEdge.BottomLeft || edge == ResizeEdge.BottomRight)
                {
                    var adjustedMargin = 1d - mousePercentage.Y;
                    margins.Bottom = minimumSize + adjustedMargin + margins.Top < 1d ? adjustedMargin : margins.Bottom;
                }

                MarginPercentages = margins;

                UpdateElementPercentages();
            }
        }

        private void SetMarginPercentages(Thickness thickness)
        {
            Column_Width_Left.Width = new GridLength(thickness.Left, GridUnitType.Star);
            Column_Width_Right.Width = new GridLength(thickness.Right, GridUnitType.Star);
            Row_Height_Above.Height = new GridLength(thickness.Top, GridUnitType.Star);
            Row_Height_Below.Height = new GridLength(thickness.Bottom, GridUnitType.Star);
        }

        private void UpdateElementPercentages()
        {
            var width = 1d - (Column_Width_Left.Width.Value + Column_Width_Right.Width.Value);
            var height = 1d - (Row_Height_Above.Height.Value + Row_Height_Below.Height.Value);

            Column_Width_Element.Width = new GridLength(width, GridUnitType.Star);
            Row_Height_Element.Height = new GridLength(height, GridUnitType.Star);
        }

        private Point GetMouseLocationAsPercentage()
        {
            var position = Mouse.GetPosition(this);
            var overallSize = new Size(ActualWidth, ActualHeight);

            return new Point(Math.Clamp(position.X / overallSize.Width, 0d, 1d), Math.Clamp(position.Y / overallSize.Height, 0d, 1d));
        }

        private void SetDisplayElement(IDisplayElement element)
        {
            if (element != null && element is UIElement uiElement)
            {
                ContentPresenter_Element.Content = uiElement;
            }
        }

        private bool PointWithin(double point, double min, double max) => point > min && point < max;

        private Thickness GetMarginPercentages() => new Thickness(Column_Width_Left.Width.Value, Row_Height_Above.Height.Value, Column_Width_Right.Width.Value, Row_Height_Below.Height.Value);

        private Thickness NegateThickness(Thickness thickness) => new Thickness(0 - thickness.Left, 0 - thickness.Top, 0 - thickness.Right, 0 - thickness.Bottom);
    }
}
