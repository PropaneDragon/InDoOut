using InDoOut_Core.Functions;
using InDoOut_Core.Logging;
using InDoOut_Desktop.Programs;
using InDoOut_Desktop.UI.Windows;
using InDoOut_Executable_Core.Programs;
using InDoOut_Json_Storage;
using InDoOut_Plugins.Loaders;
using InDoOut_UI_Common.Events;
using InDoOut_UI_Common.InterfaceElements;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace InDoOut_Desktop.UI.Controls.Sidebar
{
    public partial class Sidebar : UserControl
    {
        private readonly TimeSpan _animationTime = TimeSpan.FromMilliseconds(500);
        private readonly DispatcherTimer _updateTimer = new DispatcherTimer(DispatcherPriority.Normal);

        private bool _collapsed = false;
        private ITaskView _taskView = null;
        private ICommonProgramDisplay _programDisplay = null;

        public bool Collapsed { get => _collapsed; set { if (value) Collapse(); else Expand(); } }
        public ITaskView TaskView { get => _taskView; set => TaskViewChanged(value); }
        public ICommonProgramDisplay ProgramDisplay { get => _programDisplay; set => ProgramDisplayChanged(value); }

        public Sidebar()
        {
            InitializeComponent();

            _updateTimer.Interval = TimeSpan.FromMilliseconds(500);
            _updateTimer.Start();
            _updateTimer.Tick += UpdateTimer_Tick;
        }

        public void Collapse()
        {
            if (!_collapsed)
            {
                var offset = -(ActualWidth - ColumnDefinition_Extended.Width.Value);
                var easingFunction = new ExponentialEase() { EasingMode = EasingMode.EaseInOut };
                var sidebarAnimation = new ThicknessAnimation(new Thickness(offset, 0, 0, 0), _animationTime) { EasingFunction = easingFunction };
                var fadeOutAnimation = new DoubleAnimation(0, _animationTime) { EasingFunction = easingFunction };

                BeginAnimation(MarginProperty, sidebarAnimation);
                Grid_CollapsibleContent.BeginAnimation(OpacityProperty, fadeOutAnimation);

                sidebarAnimation.Completed += (sender, e) => Grid_CollapsibleContent.Visibility = Visibility.Hidden;                
            }

            _collapsed = true;
        }

        public void Expand()
        {
            if (_collapsed)
            {
                Grid_CollapsibleContent.Visibility = Visibility.Visible;

                var easingFunction = new ExponentialEase() { EasingMode = EasingMode.EaseInOut };
                var sidebarAnimation = new ThicknessAnimation(new Thickness(0, 0, 0, 0), _animationTime) { EasingFunction = easingFunction };
                var opacityAnimation = new DoubleAnimation(1, _animationTime) { EasingFunction = easingFunction }; ;

                BeginAnimation(MarginProperty, sidebarAnimation);
                Grid_CollapsibleContent.BeginAnimation(OpacityProperty, opacityAnimation);
            }

            _ = Focus();

            _collapsed = false;
        }

        private void TaskViewChanged(ITaskView taskView)
        {
            if (_taskView != null)
            {
                _taskView.OnProgramDisplayChanged -= TaskView_OnProgramDisplayChanged;
            }

            _taskView = taskView;

            if (_taskView != null)
            {
                _taskView.OnProgramDisplayChanged += TaskView_OnProgramDisplayChanged;
                ProgramDisplay = _taskView.CurrentProgramDisplay;
            }
        }

        private void TaskView_OnProgramDisplayChanged(object sender, CurrentProgramDisplayEventArgs e)
        {
            ProgramDisplay = e.ProgramDisplay;
        }

        private void ProgramDisplayChanged(ICommonProgramDisplay programDisplay)
        {
            _programDisplay = programDisplay;

            ItemList_Functions.FunctionView = programDisplay;

            UpdatePlayStopButtons();
        }

        private void UpdatePlayStopButtons()
        {
            if (_programDisplay != null)
            {
                var programRunning = _programDisplay?.AssociatedProgram?.Running ?? false;
                var programStopping = _programDisplay?.AssociatedProgram?.Stopping ?? false;

                Button_RunProgram.Visibility = !programStopping && !programRunning ? Visibility.Visible : Visibility.Hidden;
                Button_StopProgram.Visibility = !programStopping && programRunning ? Visibility.Visible : Visibility.Hidden;
                Button_ProgramStopping.Visibility = programStopping ? Visibility.Visible : Visibility.Hidden;
            }
        }

        private void UpdateTimer_Tick(object sender, EventArgs e)
        {
            UpdatePlayStopButtons();
        }

        private void Button_Collapse_Click(object sender, RoutedEventArgs e)
        {
            Collapsed = !Collapsed;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Collapsed = true;
        }

        private void SearchBar_SearchRequested(object sender, InDoOut_UI_Common.Controls.Search.SearchArgs e)
        {
            Log.Instance.Header("Search requested");
            Log.Instance.Info("Search query: ", e?.Query);

            ItemList_Functions.Filter(e.Query);
        }

        private void Button_SwitchMode_Click(object sender, RoutedEventArgs e)
        {
            Log.Instance.Header("Switch button clicked");

            if (_programDisplay != null)
            {
                _programDisplay.CurrentViewMode = _programDisplay.CurrentViewMode == ProgramViewMode.IO ? ProgramViewMode.Variables : ProgramViewMode.IO;
            }
        }

        private void Button_RunProgram_Click(object sender, RoutedEventArgs e)
        {
            Log.Instance.Header("Run button clicked");

            if ((!_programDisplay?.AssociatedProgram?.Running ?? false) && (!_programDisplay.AssociatedProgram?.Stopping ?? false))
            {
                _programDisplay?.AssociatedProgram?.Trigger(null);
            }

            UpdatePlayStopButtons();
        }

        private void Button_StopProgram_Click(object sender, RoutedEventArgs e)
        {
            Log.Instance.Header("Stop button clicked");

            _programDisplay?.AssociatedProgram?.Stop();

            UpdatePlayStopButtons();
        }

        private void Button_NewProgram_Click(object sender, RoutedEventArgs e)
        {
            Log.Instance.Header("New button clicked");

            if (_programDisplay != null)
            {
                _ = ProgramHolder.Instance.RemoveProgram(_programDisplay?.AssociatedProgram);
                _programDisplay.AssociatedProgram = ProgramHolder.Instance.NewProgram();
            }
        }

        private async void Button_OpenProgram_Click(object sender, RoutedEventArgs e)
        {
            Log.Instance.Header("Open button clicked");

            if (_programDisplay != null)
            {
                var program = await ProgramSaveLoad.Instance.LoadProgramDialogAsync(ProgramHolder.Instance, new ProgramJsonStorer(new FunctionBuilder(), LoadedPlugins.Instance), Window.GetWindow(this));
                if (program != null)
                {
                    _ = ProgramHolder.Instance.RemoveProgram(_programDisplay?.AssociatedProgram);
                    _programDisplay.AssociatedProgram = program;
                }
            }
        }

        private async void Button_SaveProgram_Click(object sender, RoutedEventArgs e)
        {
            Log.Instance.Header("Save button clicked");

            _ = await ProgramSaveLoad.Instance.TrySaveProgramFromMetadataAsync(_programDisplay?.AssociatedProgram, new ProgramJsonStorer(new FunctionBuilder(), LoadedPlugins.Instance), Window.GetWindow(this));
        }

        private async void Button_SaveProgramAs_Click(object sender, RoutedEventArgs e)
        {
            Log.Instance.Header("Save as button clicked");

            _ = await ProgramSaveLoad.Instance.SaveProgramDialogAsync(_programDisplay?.AssociatedProgram, new ProgramJsonStorer(new FunctionBuilder(), LoadedPlugins.Instance), Window.GetWindow(this));
        }

        private void Button_TaskViewer_Click(object sender, RoutedEventArgs e)
        {
            Log.Instance.Header("Task viewer button clicked");

            TaskView?.ShowTasks();
            Collapse();
        }

        private void Button_Settings_Click(object sender, RoutedEventArgs e)
        {
            Log.Instance.Header("Settings button clicked");

            var settingsWindow = new SettingsWindow()
            {
                Owner = Window.GetWindow(this)
            };

            settingsWindow.Show();
            Collapse();
        }
    }
}
