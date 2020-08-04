using InDoOut_Core.Entities.Programs;
using InDoOut_Core.Options.Types;
using InDoOut_Executable_Core.Options;
using InDoOut_Executable_Core.Programs;
using InDoOut_UI_Common.Events;
using InDoOut_UI_Common.InterfaceElements;
using InDoOut_UI_Common.SaveLoad;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace InDoOut_UI_Common.Controls.TaskManager
{
    public partial class TaskView : UserControl, ITaskView
    {
        public static readonly string TASK_STARTUP_PROGRAM_OPTION_NAME = "Startup programs";

        private ICommonProgramDisplay _programDisplay = null;

        public IProgramDisplayCreator ProgramDisplayCreator { get; set; } = new NullProgramDisplayCreator();
        public ICommonProgramDisplay CurrentProgramDisplay { get => _programDisplay; set => ProgramDisplayChanged(value); }

        public event EventHandler<CurrentProgramDisplayEventArgs> OnProgramDisplayChanged;

        public TaskView()
        {
            InitializeComponent();
        }

        public void CreateNewTask(bool bringToFront = false)
        {
            CreateNewTask(ProgramHolder.Instance.NewProgram(), bringToFront);
        }

        public async Task<bool> CreateNewTask(string path, bool runAutomatically = false, bool bringToFront = false)
        {
            var program = await CommonProgramSaveLoad.Instance.LoadProgramAsync(path);
            if (program != null && ProgramHolder.Instance.ProgramExists(program))
            {
                if (runAutomatically)
                {
                    program.Trigger(null);
                }

                CreateNewTask(program, bringToFront);

                return true;
            }

            return false;
        }

        public void CreateNewTask(IProgram program, bool bringToFront = false)
        {
            if (program != null)
            {
                var programDisplay = ProgramDisplayCreator?.CreateProgramDisplay(program);
                if (programDisplay != null)
                {
                    var taskItem = new TaskItem(programDisplay, this);
                    var fadeInAnimation = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(600)) { EasingFunction = new CircleEase() { EasingMode = EasingMode.EaseInOut } };

                    _ = Wrap_Tasks.Children.Add(taskItem);

                    if (bringToFront)
                    {
                        BringToFront(taskItem);
                    }
                    else
                    {
                        taskItem.BeginAnimation(OpacityProperty, fadeInAnimation);
                    }
                }
            }
        }

        public void ShowTasks()
        {
            foreach (var child in Wrap_Tasks.Children)
            {
                if (child is TaskItem taskItem && taskItem.ProgramDisplay == CurrentProgramDisplay)
                {
                    taskItem.UpdateSnapshotWithTransition();
                }
            }

            var fadeInAnimation = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(500)) { EasingFunction = new CircleEase() { EasingMode = EasingMode.EaseInOut } };
            var fadeOutAnimation = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(500)) { EasingFunction = new CircleEase() { EasingMode = EasingMode.EaseInOut } };
            var contractAnimation = new DoubleAnimation(1, 0.1, TimeSpan.FromMilliseconds(500)) { EasingFunction = new CircleEase() { EasingMode = EasingMode.EaseInOut } };

            fadeOutAnimation.Completed += (sender, e) => Grid_CurrentHost.Visibility = Visibility.Hidden;

            Grid_CurrentHost.BeginAnimation(OpacityProperty, fadeOutAnimation);

            Border_ScaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, contractAnimation);
            Border_ScaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, contractAnimation);

            Grid_Tasks.Visibility = Visibility.Visible;
            Grid_Tasks.BeginAnimation(OpacityProperty, fadeInAnimation);

            CurrentProgramDisplay = null;
        }

        public void BringToFront(ITaskItem taskItem)
        {
            if (taskItem?.ProgramDisplay != null)
            {
                BringToFront(taskItem.ProgramDisplay);
            }
        }

        public void BringToFront(ICommonProgramDisplay programDisplay)
        {
            if (programDisplay != null && programDisplay is UIElement uiElement)
            {
                Grid_CurrentHost.Visibility = Visibility.Visible;

                Border_CurrentHost.Child = uiElement;

                if (_programDisplay != null)
                {
                    var fadeInAnimation = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(500)) { EasingFunction = new CircleEase() { EasingMode = EasingMode.EaseInOut } };
                    var fadeOutAnimation = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(500)) { EasingFunction = new CircleEase() { EasingMode = EasingMode.EaseInOut } };
                    var expandAnimation = new DoubleAnimation(0.1, 1, TimeSpan.FromMilliseconds(500)) { EasingFunction = new CircleEase() { EasingMode = EasingMode.EaseInOut } };

                    fadeOutAnimation.Completed += (sender, e) => Grid_Tasks.Visibility = Visibility.Hidden;

                    Border_ScaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, expandAnimation);
                    Border_ScaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, expandAnimation);
                    Grid_CurrentHost.BeginAnimation(OpacityProperty, fadeInAnimation);
                    Grid_Tasks.BeginAnimation(OpacityProperty, fadeOutAnimation);
                }
                else
                {
                    Grid_Tasks.Visibility = Visibility.Hidden;
                    Grid_CurrentHost.Opacity = 1;
                }

                CurrentProgramDisplay = programDisplay;
            }
        }

        public bool RemoveTask(ITaskItem task)
        {
            if (task != null)
            {
                foreach (UIElement child in Wrap_Tasks.Children)
                {
                    if (child is ITaskItem item && item == task)
                    {
                        var program = task?.ProgramDisplay?.AssociatedProgram;
                        if (program != null)
                        {
                            program.Stop();
                        }

                        Wrap_Tasks.Children.Remove(child);

                        return true;
                    }
                }
            }

            return false;
        }

        public async Task<bool> LoadStoredOptionTasks(bool runAutomatically = true)
        {
            var optionHolder = ProgramOptionsHolder.Instance?.ProgramOptions?.OptionHolder;
            var options = optionHolder?.Options;
            var allLoaded = true;

            if (options != null)
            {
                var foundOption = options.Find(option => option.Name == TASK_STARTUP_PROGRAM_OPTION_NAME);

                if (foundOption is HiddenListOption listOption && (listOption?.ListValue?.Count ?? 0) > 0)
                {
                    foreach (var item in listOption.ListValue)
                    {
                        allLoaded = await CreateNewTask(item, runAutomatically, false) && allLoaded;
                    }
                }
            }

            return allLoaded;
        }

        private void ProgramDisplayChanged(ICommonProgramDisplay programDisplay)
        {
            if (programDisplay != _programDisplay)
            {
                _programDisplay = programDisplay;

                if (programDisplay != null)
                {
                    BringToFront(programDisplay);
                }

                OnProgramDisplayChanged?.Invoke(this, new CurrentProgramDisplayEventArgs(programDisplay));
            }
        }

        private void Button_NewTask_Click(object sender, RoutedEventArgs e)
        {
            CreateNewTask();
        }
    }
}
