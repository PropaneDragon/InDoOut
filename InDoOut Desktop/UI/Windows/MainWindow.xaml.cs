using InDoOut_Core.Logging;
using InDoOut_Desktop.Loading;
using InDoOut_Desktop.Options;
using InDoOut_Desktop.Programs;
using InDoOut_Desktop.UI.Threading;
using InDoOut_Executable_Core.Location;
using InDoOut_Executable_Core.Logging;
using InDoOut_Executable_Core.Messaging;
using InDoOut_Executable_Core.Options;
using InDoOut_UI_Common.Messaging;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace InDoOut_Desktop.UI.Windows
{
    public partial class MainWindow : Window
    {
        private readonly DispatcherTimer _titleTimer = new DispatcherTimer(DispatcherPriority.Background);
        private readonly LogFileSaver _logSaver = new LogFileSaver(StandardLocations.Instance);

        public MainWindow()
        {
            InitializeComponent();

            Application.Current.DispatcherUnhandledException += Application_DispatcherUnhandledException;
            ProgramOptionsHolder.Instance.ProgramOptions = new ProgramOptions();
            UIThread.Instance.SetCurrentThreadAsUIThread();
            UserMessageSystemHolder.Instance.CurrentUserMessageSystem = new DesktopUserMessageSystem();

            _titleTimer.Interval = TimeSpan.FromMilliseconds(300);
            _titleTimer.Start();
            _titleTimer.Tick += UpdateTimer_Tick;

            _logSaver.BeginAutoSave();
        }

        private async Task FinishLoading()
        {
            var sidebar = Sidebar_Main;
            var taskView = TaskView_Main;

            if (taskView != null)
            {
                taskView.ProgramDisplayCreator = new ProgramDisplayCreator();

                _ = await taskView.LoadStoredOptionTasks(true);

                taskView.CreateNewTask(true);
            }

            if (sidebar != null)
            {
                sidebar.TaskView = taskView;
            }

            if (ProgramOptionsHolder.Instance.Get<ProgramOptions>()?.StartInBackground.Value ?? false)
            {
                WindowState = WindowState.Minimized;
            }
            else
            {
                _ = Activate();
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
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Log.Instance.Header("Application closing");

            var saveAttempts = 0;

            while ((!_logSaver?.SaveLog() ?? false) && saveAttempts++ < 3)
            {
                Thread.Sleep(TimeSpan.FromMilliseconds(100));
            }
        }

        private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            Log.Instance.Header("Application crash!");
            Log.Instance.Error(e?.Exception?.Message ?? "null");
            Log.Instance.Error(e?.Exception?.StackTrace ?? "null");

            var saveAttempts = 0;

            while ((!_logSaver?.SaveLog() ?? false) && saveAttempts++ < 3)
            {
                Thread.Sleep(TimeSpan.FromMilliseconds(100));
            }
        }

        private void UpdateTimer_Tick(object sender, EventArgs e)
        {
            var programName = "No program";
            var program = TaskView_Main.CurrentProgramDisplay?.AssociatedProgram;

            if (program != null)
            {
                programName = string.IsNullOrEmpty(program.Name) ? "Untitled" : program.Name;
            }

            Title = $"{programName}";
        }
    }
}
