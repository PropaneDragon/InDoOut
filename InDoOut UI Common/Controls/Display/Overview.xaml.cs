using InDoOut_UI_Common.InterfaceElements;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace InDoOut_UI_Common.Controls.Display
{
    public partial class Overview : UserControl
    {
        private readonly DispatcherTimer _updateTimer = new(DispatcherPriority.Render);

        private ICommonBlockDisplay _display = null;

        public Size TotalSize => _display?.TotalSize ?? new Size();
        public Size ViewSize => _display?.ViewSize ?? new Size();
        public Vector ActualSizeToOverviewRatio => new(ActualWidth / TotalSize.Width, ActualHeight / TotalSize.Height);
        public Vector OverviewToActualSizeRatio => new(TotalSize.Width / ActualWidth, TotalSize.Height / ActualHeight);
        public ICommonBlockDisplay Display { get => _display; set => ChangeBlockView(value); }

        public Overview()
        {
            InitializeComponent();

            _updateTimer.Interval = TimeSpan.FromMilliseconds(333);
            _updateTimer.Tick += UpdateTimer_Tick;
            _updateTimer.Start();

            UpdatePositions();
        }

        private void ChangeBlockView(ICommonBlockDisplay blockView)
        {
            if (_display != null)
            {
                //Todo: Teardown old display
            }

            _display = blockView;

            if (_display != null)
            {
                UpdatePositions();
            }
        }

        private void UpdatePositions()
        {
            if (_display != null)
            {
                var topLeftViewCoordinate = _display.TopLeftViewCoordinate;
                var viewportRect = new Rect(topLeftViewCoordinate, ViewSize);
                var ratioVector = ActualSizeToOverviewRatio;
                var adjustedViewportSize = AdjustByRatio(viewportRect, ratioVector);

                Rectangle_Viewport.Width = adjustedViewportSize.Width;
                Rectangle_Viewport.Height = adjustedViewportSize.Height;

                Canvas.SetLeft(Rectangle_Viewport, adjustedViewportSize.X);
                Canvas.SetTop(Rectangle_Viewport, adjustedViewportSize.Y);

                Canvas_Layout.Children.Clear();

                var functions = _display.UIFunctions;
                foreach (var function in functions)
                {
                    if (function is FrameworkElement element)
                    {
                        var position = _display.GetPosition(element);
                        var size = new Size(element.ActualWidth, element.ActualHeight);
                        var functionRect = new Rect(position, size);
                        var adjustedFunctionRect = AdjustByRatio(functionRect, ratioVector);

                        var viewRectangle = new Rectangle()
                        {
                            Width = adjustedFunctionRect.Width,
                            Height = adjustedFunctionRect.Height,
                            Stroke = new SolidColorBrush(Color.FromArgb(100, 255, 255, 255)),
                            StrokeThickness = 1,
                            RadiusX = 2,
                            RadiusY = 2
                        };

                        _ = Canvas_Layout.Children.Add(viewRectangle);

                        Canvas.SetLeft(viewRectangle, adjustedFunctionRect.X);
                        Canvas.SetTop(viewRectangle, adjustedFunctionRect.Y);
                    }
                }
            }
        }

        private Point AdjustByRatio(Point point, Vector ratio) => new(point.X * ratio.X, point.Y * ratio.Y);

        private Size AdjustByRatio(Size size, Vector ratio) => new(size.Width * ratio.X, size.Height * ratio.Y);

        private Rect AdjustByRatio(Rect rect, Vector ratio) => new(AdjustByRatio(rect.Location, ratio), AdjustByRatio(rect.Size, ratio));

        private void UpdateTimer_Tick(object sender, EventArgs e) => UpdatePositions();

        private void UserControl_PreviewMouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (Display != null && Display is IScrollable scrollable)
            {
                var clickedPosition = e.GetPosition(this);
                var ratioClickedPosition = AdjustByRatio(clickedPosition, OverviewToActualSizeRatio);
                var centrePoint = new Point(ratioClickedPosition.X - (ViewSize.Width / 2d), ratioClickedPosition.Y - (ViewSize.Height / 2d));

                scrollable.Offset = centrePoint;
            }
        }
    }
}
