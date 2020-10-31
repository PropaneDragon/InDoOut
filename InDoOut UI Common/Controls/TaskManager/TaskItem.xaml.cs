using InDoOut_Core.Options.Types;
using InDoOut_Executable_Core.Messaging;
using InDoOut_Executable_Core.Options;
using InDoOut_Executable_Core.Storage;
using InDoOut_UI_Common.InterfaceElements;
using InDoOut_UI_Common.SaveLoad;
using System;
using System.Threading.Tasks;
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

        private bool AssociatedProgramIsStartupProgram => GetStartupProgramsOption()?.ListValue?.Contains(GetAssociatedProgramLocation() ?? "") ?? false;

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
            Button_AddStartWithProgram.Visibility = !AssociatedProgramIsStartupProgram ? Visibility.Visible : Visibility.Collapsed;
            Button_RemoveStartWithProgram.Visibility = AssociatedProgramIsStartupProgram ? Visibility.Visible : Visibility.Collapsed;
        }

        private HiddenListOption GetStartupProgramsOption()
        {
            var optionHolder = ProgramOptionsHolder.Instance?.ProgramOptions?.OptionHolder;
            var options = optionHolder?.Options;

            if (options != null)
            {
                var foundOption = options.Find(option => option.Name == TaskManager.TaskView.TASK_STARTUP_PROGRAM_OPTION_NAME);

                if (foundOption is HiddenListOption listOption)
                {
                    return listOption;
                }
            }

            return null;
        }

        private string GetAssociatedProgramLocation()
        {
            var metadata = ProgramDisplay?.AssociatedProgram?.Metadata;

            return metadata != null && metadata.TryGetValue(ProgramStorer.PROGRAM_METADATA_LAST_LOADED_FROM, out var programLocation) ? programLocation : null;
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
            var programLocation = GetAssociatedProgramLocation();
            var startupOption = GetStartupProgramsOption();

            if (startupOption != null)
            {
                if (!string.IsNullOrEmpty(programLocation))
                {
                    var list = startupOption.ListValue;

                    if (AssociatedProgramIsStartupProgram)
                    {
                        _ = list.RemoveAll(value => value == programLocation);
                    }
                    else
                    {
                        list.Add(programLocation);
                    }

                    startupOption.ListValue = list;
                }
                else
                {
                    UserMessageSystemHolder.Instance.CurrentUserMessageSystem?.ShowError("Can't add an unsaved program", "The program needs to be saved first in order to be added to the startup list.");
                }
            }
            else
            {
                UserMessageSystemHolder.Instance.CurrentUserMessageSystem?.ShowError("Program couldn't be added to startup list", "There appears to be an issue with the options, and the program couldn't be added to the options.");
            }

            UpdateProgramState();

            _ = Task.Run(async () => await CommonOptionsSaveLoad.Instance.SaveProgramOptionsAsync());
        }

        private void Button_RunTask_Click(object sender, RoutedEventArgs e) => ProgramDisplay?.AssociatedProgram?.Trigger(null);

        private void Button_StopTask_Click(object sender, RoutedEventArgs e) => ProgramDisplay?.AssociatedProgram?.Stop();
    }
}
