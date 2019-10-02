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
            UpdateProgramName();

            Dock_HiddenContent.Opacity = 0;
        }

        public void UpdateSnapshot()
        {
            if (BlockView != null && BlockView is UIElement uiElement && uiElement.RenderSize.Width > 0 && uiElement.RenderSize.Height > 0)
            {
                var captureSize = uiElement.RenderSize;
                var captureRectangle = new Rect(captureSize);
                var drawingVisual = new DrawingVisual();

                using (var drawingContext = drawingVisual.RenderOpen())
                {
                    uiElement.InvalidateVisual();
                    drawingContext.DrawRectangle(new VisualBrush(uiElement) { Stretch = Stretch.UniformToFill, AutoLayoutContent = true }, null, captureRectangle);
                }

                var bitmap = new RenderTargetBitmap((int)captureSize.Width, (int)captureSize.Height, 96, 96, PixelFormats.Default);
                bitmap.Render(drawingVisual);

                Image_Preview.Source = bitmap;

                Image_ScaleTransform.ScaleX = 5;
                Image_ScaleTransform.ScaleY = 5;

                var contractAnimation = new DoubleAnimation(1, TimeSpan.FromMilliseconds(500)) { EasingFunction = new CircleEase() { EasingMode = EasingMode.EaseInOut } };

                Image_ScaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, contractAnimation);
                Image_ScaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, contractAnimation);
            }

            UpdateProgramName();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateSnapshot();
            UpdateProgramName();
        }

        private void UpdateProgramName()
        {
            if (BlockView != null)
            {
                Text_ProgramName.Text = BlockView?.AssociatedProgram?.Name ?? "Untitled";
            }
        }

        private void UserControl_PreviewMouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ChangedButton == System.Windows.Input.MouseButton.Left)
            {
                TaskView?.BringToFront(this);

                var expandAnimation = new DoubleAnimation(5, TimeSpan.FromMilliseconds(500)) { EasingFunction = new CircleEase() { EasingMode = EasingMode.EaseInOut } };

                Image_ScaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, expandAnimation);
                Image_ScaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, expandAnimation);
            }
        }

        private void UserControl_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var fadeInAnimation = new DoubleAnimation(1, TimeSpan.FromMilliseconds(300)) { EasingFunction = new CircleEase() { EasingMode = EasingMode.EaseInOut } };

            Dock_HiddenContent.BeginAnimation(OpacityProperty, fadeInAnimation);
        }

        private void UserControl_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var fadeOutAnimation = new DoubleAnimation(0, TimeSpan.FromMilliseconds(300)) { EasingFunction = new CircleEase() { EasingMode = EasingMode.EaseInOut } };

            Dock_HiddenContent.BeginAnimation(OpacityProperty, fadeOutAnimation);
        }
    }
}
