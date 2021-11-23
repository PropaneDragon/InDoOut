using InDoOut_Display.Loading;
using InDoOut_Display.Options;
using InDoOut_Display.Programs;
using InDoOut_Executable_Core.Location;
using InDoOut_Executable_Core.Logging;
using InDoOut_Executable_Core.Messaging;
using InDoOut_Executable_Core.Options;
using InDoOut_UI_Common.Controls.Screens;
using InDoOut_UI_Common.Messaging;
using InDoOut_UI_Common.SaveLoad;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace InDoOut_Display.UI.Windows
{
    public partial class MainWindow : Window
    {
        private readonly LogFileSaver _logSaver = new(StandardLocations.Instance);
        private readonly DispatcherTimer _titleTimer = new(DispatcherPriority.Background);
        private readonly DispatcherTimer _windowStateSaveTimer = new(DispatcherPriority.Background);

        private bool _windowStateSavingEnabled = false;
        private bool _windowStateChanged = false;

        public MainWindow()
        {
            InitializeComponent();

            _logSaver.BeginAutoSave();

            ProgramOptionsHolder.Instance.ProgramOptions = new ProgramOptions();
            UserMessageSystemHolder.Instance.CurrentUserMessageSystem = new DesktopUserMessageSystem();

            _titleTimer.Interval = TimeSpan.FromMilliseconds(300);
            _titleTimer.Start();
            _titleTimer.Tick += TitleTimer_Tick;

            _windowStateSaveTimer.Interval = TimeSpan.FromMilliseconds(300);
            _windowStateSaveTimer.Start();
            _windowStateSaveTimer.Tick += WindowStateSaveTimer_Tick;

            Sidebar_Main.AssociatedTaskView = TaskView_Main;
        }

        private async Task FinishLoading()
        {
            var taskView = TaskView_Main;

            if (taskView != null)
            {
                taskView.ProgramDisplayCreator = new ProgramDisplayCreator();

                _ = await taskView.LoadStoredOptionTasks(true);

                taskView.CreateNewTask(true);
            }

            SetWindowFromOptions();

            _windowStateChanged = false;
            _windowStateSavingEnabled = true;
        }

        private void SetWindowFromOptions()
        {
            var programOptions = ProgramOptionsHolder.Instance.Get<ProgramOptions>();
            if (programOptions != null)
            {
                Left = programOptions.LastWindowX?.ValueAs(0d) ?? 0d;
                Top = programOptions.LastWindowY?.ValueAs(0d) ?? 0d;
                Width = programOptions.LastWindowWidth?.ValueAs(100d) ?? 100d;
                Height = programOptions.LastWindowHeight?.ValueAs(100d) ?? 100d;
                WindowState = (programOptions.LastWindowMaximised?.ValueAs(true) ?? true) ? WindowState.Maximized : WindowState.Normal;
            }
        }

        private void SetOptionsFromWindow()
        {
            if (_windowStateSavingEnabled)
            {
                var programOptions = ProgramOptionsHolder.Instance.Get<ProgramOptions>();
                if (programOptions != null)
                {
                    _ = programOptions.LastWindowX?.ValueFrom(Left);
                    _ = programOptions.LastWindowY?.ValueFrom(Top);
                    _ = programOptions.LastWindowWidth?.ValueFrom(Width);
                    _ = programOptions.LastWindowHeight?.ValueFrom(Height);
                    _ = programOptions.LastWindowMaximised?.ValueFrom(WindowState == WindowState.Maximized);
                }
            }
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _ = Activate();

            if (await Splash_Overlay.RunTaskAsync(new MainWindowLoadingTask()))
            {
                await FinishLoading();
                return;
            }

            Close();
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            SetOptionsFromWindow();

            _windowStateChanged = true;
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            SetOptionsFromWindow();

            _windowStateChanged = true;
        }

        private void TitleTimer_Tick(object sender, EventArgs e)
        {
            var programName = "No program";
            var program = TaskView_Main.CurrentProgramDisplay?.AssociatedProgram;

            if (program != null)
            {
                programName = string.IsNullOrEmpty(program.Name) ? "Untitled" : program.Name;
            }

            Title = $"{programName}";
        }

        private async void WindowStateSaveTimer_Tick(object sender, EventArgs e)
        {
            _windowStateSaveTimer.Stop();

            if (_windowStateSavingEnabled && _windowStateChanged)
            {
                _windowStateChanged = false;

                _ = await CommonOptionsSaveLoad.Instance.SaveProgramOptionsAsync();
            }

            _windowStateSaveTimer.Start();
        }
    }
}
