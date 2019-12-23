using InDoOut_Display.Actions.Resizing;
using InDoOut_Display.Actions.Scaling;
using InDoOut_Display.Actions.Selecting;
using InDoOut_Display.UI.Controls.Screens;
using InDoOut_Display_Core.Elements;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace InDoOut_Display.UI.Controls.DisplayElement
{
    public partial class DisplayElementContainer : UserControl, IResizable, IScalable, IScreenSelectable
    {
        private static readonly Thickness THICKNESS_STATIC = new Thickness(1);
        private static readonly Thickness THICKNESS_SELECTED = new Thickness(2);

        private bool _selected = false;
        private bool _resizing = false;
        private Thickness _originalMargins = new Thickness();

        public bool AutoScale { get; set; } = false;
        public double Scale { get; set; } = 1d;
        public IDisplayElement AssociatedDisplayElement { get => ContentPresenter_Element.Content as IDisplayElement; set => SetDisplayElement(value); }
        public Size Size => new Size(Border_Presenter.ActualWidth, Border_Presenter.ActualHeight);
        public Thickness MarginPercentages { get => GetMarginPercentages(); set => SetMarginPercentages(value); }

        public DisplayElementContainer(IDisplayElement element = null)
        {
            InitializeComponent();

            AssociatedDisplayElement = element;
        }

        public bool CanResize(IScreen screen) => true;

        public bool CanScale(IScreen screen) => true;

        public bool CanSelect(IScreen view) => true;

        public void ScaleChanged(IScreen screen)
        {
        }

        public void SelectionStarted(IScreen view)
        {
            _selected = true;
            UpdateBorder();
        }

        public void SelectionEnded(IScreen view)
        {
            _selected = false;
            UpdateBorder();
        }

        public void ResizeStarted(IScreen screen)
        {
            _resizing = true;
            _originalMargins = MarginPercentages;
            UpdateBorder();
        }

        public void ResizeEnded(IScreen screen)
        {
            _resizing = false;
            UpdateBorder();
        }

        public bool CloseToEdge(IScreen screen, Point point, double distance = 5) => GetCloseEdge(screen, point, distance) != ResizeEdge.None;

        public ResizeEdge GetCloseEdge(IScreen screen, Point point, double distance = 5)
        {            
            var size = Size;
            var localPoint = TranslatePoint(point, Border_Presenter);
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

        public void ResizeMoved(IScreen screen, ResizeEdge edge, Point delta)
        {
            if (screen != null && edge.ValidEdge())
            {
                var deltaPercentage = GetDeltaAsPercentage(delta);
                var margins = _originalMargins;
                var currentMargins = MarginPercentages;
                var minimumSize = 0.01;
                var individualEdges = edge.IndividualEdges();

                foreach (var individualEdge in individualEdges)
                {
                    var oppositeEdge = individualEdge.OppositeEdge();
                    var deltaForEdge = GetDeltaForEdge(individualEdge, deltaPercentage);
                    var adjustedMargin = GetMarginForEdge(individualEdge, margins) + deltaForEdge;
                    var oppositeMargin = GetMarginForEdge(oppositeEdge, margins);
                    var currentEdgeMargin = GetMarginForEdge(individualEdge, currentMargins);
                    var totalMargin = adjustedMargin + minimumSize + oppositeMargin;
                    var validMargin = totalMargin < 1d && adjustedMargin > minimumSize;

                    SetMarginForEdge(individualEdge, validMargin ? adjustedMargin : currentEdgeMargin, ref margins);
                }

                MarginPercentages = margins;

                UpdateElementPercentages();
            }
        }

        private double GetDeltaForEdge(ResizeEdge edge, Point delta)
        {
            return edge switch
            {
                ResizeEdge.Left => -delta.X,
                ResizeEdge.Top => -delta.Y,
                ResizeEdge.Right => delta.X,
                ResizeEdge.Bottom => delta.Y,

                _ => 0
            };
        }

        private double GetMarginForEdge(ResizeEdge edge, Thickness margin)
        {
            return edge switch
            {
                ResizeEdge.Left => margin.Left,
                ResizeEdge.Top => margin.Top,
                ResizeEdge.Right => margin.Right,
                ResizeEdge.Bottom => margin.Bottom,

                _ => 0
            };
        }

        private void SetMarginForEdge(ResizeEdge edge, double value, ref Thickness margin)
        {
            switch (edge)
            {
                case ResizeEdge.Left:
                    margin.Left = value;
                    break;
                case ResizeEdge.Top:
                    margin.Top = value;
                    break;
                case ResizeEdge.Right:
                    margin.Right = value;
                    break;
                case ResizeEdge.Bottom:
                    margin.Bottom = value;
                    break;
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

        private Point GetDeltaAsPercentage(Point delta)
        {
            var overallSize = new Size(ActualWidth, ActualHeight);

            return new Point(delta.X / overallSize.Width, delta.Y / overallSize.Height);
        }

        private void SetDisplayElement(IDisplayElement element)
        {
            if (element != null && element is UIElement uiElement)
            {
                ContentPresenter_Element.Content = uiElement;
            }
        }

        private void UpdateBorder()
        {
            var thickness = (_selected || _resizing) ? THICKNESS_SELECTED : THICKNESS_STATIC;

            Border_Presenter.BorderThickness = thickness;
            Border_Presenter.Margin = NegateThickness(thickness);
        }

        private bool PointWithin(double point, double min, double max) => point > min && point < max;

        private Thickness GetMarginPercentages() => new Thickness(Column_Width_Left.Width.Value, Row_Height_Above.Height.Value, Column_Width_Right.Width.Value, Row_Height_Below.Height.Value);

        private Thickness NegateThickness(Thickness thickness) => new Thickness(0 - thickness.Left, 0 - thickness.Top, 0 - thickness.Right, 0 - thickness.Bottom);
    }
}
