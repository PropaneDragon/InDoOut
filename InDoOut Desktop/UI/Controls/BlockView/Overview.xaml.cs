using InDoOut_Desktop.UI.Interfaces;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace InDoOut_Desktop.UI.Controls.BlockView
{
    /// <summary>
    /// Interaction logic for Overview.xaml
    /// </summary>
    public partial class Overview : UserControl
    {
        private IBlockView _blockView = null;
        private readonly DispatcherTimer _updateTimer = new DispatcherTimer(DispatcherPriority.Normal);

        public IBlockView AssociatedBlockView { get => _blockView; set => ChangeBlockView(value); }

        public Overview()
        {
            InitializeComponent();

            _updateTimer.Interval = TimeSpan.FromMilliseconds(10);
            _updateTimer.Tick += UpdateTimer_Tick;
            _updateTimer.Start();

            UpdatePositions();
        }

        private void ChangeBlockView(IBlockView blockView)
        {
            if (_blockView != null)
            {
                //Todo: Teardown old blockview
            }

            _blockView = blockView;

            if (_blockView != null)
            {
                UpdatePositions();
            }
        }

        private void UpdatePositions()
        {
            if (_blockView != null)
            {
                var totalWindowSize = _blockView.TotalSize;
                var totalViewSize = _blockView.ViewSize;
                var topLeftViewCoordinate = _blockView.TopLeftViewCoordinate;
                var windowRect = new Rect(totalWindowSize);
                var viewportRect = new Rect(topLeftViewCoordinate, totalViewSize);

                var widthRatio = ActualWidth / totalWindowSize.Width;
                var heightRatio = ActualHeight / totalWindowSize.Height;
                var ratioVector = new Vector(widthRatio, heightRatio);

                var adjustedWindowSize = AdjustByRatio(windowRect, ratioVector);
                var adjustedViewportSize = AdjustByRatio(viewportRect, ratioVector);

                Rectangle_Viewport.Width = adjustedViewportSize.Width;
                Rectangle_Viewport.Height = adjustedViewportSize.Height;

                Canvas.SetLeft(Rectangle_Viewport, adjustedViewportSize.X);
                Canvas.SetTop(Rectangle_Viewport, adjustedViewportSize.Y);

                Canvas_Layout.Children.Clear();

                var functions = _blockView.UIFunctions;
                foreach (var function in functions)
                {
                    if (function is FrameworkElement element)
                    {
                        var position = _blockView.GetPosition(element);
                        var size = new Size(element.ActualWidth, element.ActualHeight);
                        var functionRect = new Rect(position, size);
                        var adjustedFunctionRect = AdjustByRatio(functionRect, ratioVector);

                        var viewRectangle = new Rectangle()
                        {
                            Width = adjustedFunctionRect.Width,
                            Height = adjustedFunctionRect.Height,
                            Stroke = new SolidColorBrush(Color.FromArgb(100, 255, 255, 255)),
                            StrokeThickness = 1
                        };

                        _ = Canvas_Layout.Children.Add(viewRectangle);

                        Canvas.SetLeft(viewRectangle, adjustedFunctionRect.X);
                        Canvas.SetTop(viewRectangle, adjustedFunctionRect.Y);
                    }
                }
            }
        }

        private Point AdjustByRatio(Point point, Vector ratio)
        {
            return new Point(point.X * ratio.X, point.Y * ratio.Y);
        }

        private Size AdjustByRatio(Size size, Vector ratio)
        {
            return new Size(size.Width * ratio.X, size.Height * ratio.Y);
        }

        private Rect AdjustByRatio(Rect rect, Vector ratio)
        {
            return new Rect(AdjustByRatio(rect.Location, ratio), AdjustByRatio(rect.Size, ratio));
        }

        private void UpdateTimer_Tick(object sender, EventArgs e)
        {
            UpdatePositions();
        }
    }
}
