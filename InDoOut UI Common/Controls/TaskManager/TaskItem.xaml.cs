using InDoOut_UI_Common.InterfaceElements;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace InDoOut_UI_Common.Controls.TaskManager
{
    public partial class TaskItem : UserControl, ITaskItem
    {
        private DispatcherTimer _updateTimer = null;

        public ICommonProgramDisplay ProgramDisplay { get; private set; } = null;
        public ITaskView TaskView { get; private set; } = null;

        public TaskItem()
        {
            InitializeComponent();
        }

        public TaskItem(ICommonProgramDisplay programDisplay, ITaskView taskView) : this()
        {
            ProgramDisplay = programDisplay;
            TaskView = taskView;

            UpdateSnapshotWithTransition();
            UpdateProgramName();

            Dock_HiddenContent.Opacity = 0;
        }

        public void UpdateSnapshotWithTransition()
        {
            UpdateSnapshot();

            Image_ScaleTransform.ScaleX = 5;
            Image_ScaleTransform.ScaleY = 5;

            var contractAnimation = new DoubleAnimation(1, TimeSpan.FromMilliseconds(500)) { EasingFunction = new CircleEase() { EasingMode = EasingMode.EaseInOut } };

            Image_ScaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, contractAnimation);
            Image_ScaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, contractAnimation);

            UpdateProgramName();
        }

        public void UpdateSnapshot()
        {
            if (ProgramDisplay != null && ProgramDisplay is UIElement uiElement && uiElement.RenderSize.Width > 0 && uiElement.RenderSize.Height > 0)
            {
                var captureSize = uiElement.RenderSize;
                var captureRectangle = new Rect(captureSize);
                var drawingVisual = new DrawingVisual();

                using (var drawingContext = drawingVisual.RenderOpen())
                {
                    uiElement.InvalidateVisual();
                    drawingContext.DrawRectangle(new VisualBrush(uiElement) { Stretch = Stretch.UniformToFill, AutoLayoutContent = true }, null, captureRectangle);
                    drawingContext.Close();
                }

                var bitmap = new RenderTargetBitmap((int)captureSize.Width, (int)captureSize.Height, 96, 96, PixelFormats.Default);
                bitmap.Render(drawingVisual);

                Image_Preview.Source = bitmap;
            }
        }

        private void UpdateProgramName()
        {
            if (ProgramDisplay != null)
            {
                Text_ProgramName.Text = ProgramDisplay?.AssociatedProgram?.Name ?? "Untitled";
            }
        }

        private void UpdateProgramState()
        {
            var program = ProgramDisplay?.AssociatedProgram;

            Button_RunTask.Visibility = (program != null && !program.Running && !program.Stopping) ? Visibility.Visible : Visibility.Collapsed;
            Button_StopTask.Visibility = (program != null && program.Running && !program.Stopping) ? Visibility.Visible : Visibility.Collapsed;
            Button_StoppingTask.Visibility = (program != null && !program.Running && program.Stopping) ? Visibility.Visible : Visibility.Collapsed;
        }

        private void UpdateTimer_Tick(object sender, EventArgs e)
        {
            UpdateProgramName();
            UpdateProgramState();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (_updateTimer == null)
            {
                _updateTimer = new DispatcherTimer(DispatcherPriority.Normal) { Interval = TimeSpan.FromMilliseconds(200) };
                _updateTimer.Start();
                _updateTimer.Tick += UpdateTimer_Tick;
            }

            UpdateSnapshotWithTransition();
            UpdateProgramName();
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            _updateTimer.Stop();
            _updateTimer.Tick -= UpdateTimer_Tick;
            _updateTimer = null;
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

        private void Button_RemoveTask_Click(object sender, RoutedEventArgs e)
        {
            if (TaskView != null)
            {
                _ = TaskView.RemoveTask(this);
            }
        }

        private void Button_StartWithProgram_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_RunTask_Click(object sender, RoutedEventArgs e)
        {
            ProgramDisplay?.AssociatedProgram?.Trigger(null);
        }

        private void Button_StopTask_Click(object sender, RoutedEventArgs e)
        {
            ProgramDisplay?.AssociatedProgram?.Stop();
        }
    }
}
