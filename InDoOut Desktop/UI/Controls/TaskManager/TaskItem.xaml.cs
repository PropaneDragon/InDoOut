using InDoOut_Desktop.UI.Interfaces;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace InDoOut_Desktop.UI.Controls.TaskManager
{
    public partial class TaskItem : UserControl, ITaskItem
    {
        public IBlockView BlockView { get; private set; } = null;
        public ITaskView TaskView { get; private set; } = null;

        public TaskItem()
        {
            InitializeComponent();
        }

        public TaskItem(IBlockView blockView, ITaskView taskView) : this()
        {
            BlockView = blockView;
            TaskView = taskView;

            UpdateSnapshot();
        }

        public void UpdateSnapshot()
        {
            if (BlockView != null && BlockView is UIElement uiElement && uiElement.RenderSize.Width > 0 && uiElement.RenderSize.Height > 0)
            {
                var captureSize = BlockView.ViewSize;
                var captureRectangle = new Rect(captureSize);
                var drawingVisual = new DrawingVisual();

                using (var drawingContext = drawingVisual.RenderOpen())
                {
                    drawingContext.DrawRectangle(new VisualBrush(uiElement) { AutoLayoutContent = true }, null, captureRectangle);
                }

                var bitmap = new RenderTargetBitmap((int)captureSize.Width, (int)captureSize.Height, 96, 96, PixelFormats.Default);
                bitmap.Render(drawingVisual);

                Image_Preview.Source = bitmap;

                var contractAnimation = new DoubleAnimation(1, TimeSpan.FromMilliseconds(500)) { EasingFunction = new CircleEase() { EasingMode = EasingMode.EaseInOut } };

                Image_ScaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, contractAnimation);
                Image_ScaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, contractAnimation);
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateSnapshot();
        }

        private void UserControl_PreviewMouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            TaskView?.BringToFront(this);

            var expandAnimation = new DoubleAnimation(20, TimeSpan.FromMilliseconds(500)) { EasingFunction = new CircleEase() { EasingMode = EasingMode.EaseInOut } };

            Image_ScaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, expandAnimation);
            Image_ScaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, expandAnimation);
        }
    }
}
