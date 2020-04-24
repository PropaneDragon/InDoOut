using InDoOut_Display.Loading;
using InDoOut_Display.Options;
using InDoOut_Display.Programs;
using InDoOut_Executable_Core.Location;
using InDoOut_Executable_Core.Logging;
using InDoOut_Executable_Core.Messaging;
using InDoOut_Executable_Core.Options;
using InDoOut_UI_Common.Controls.Screens;
using InDoOut_UI_Common.Messaging;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace InDoOut_Display.UI.Windows
{
    public partial class MainWindow : Window
    {
        private readonly LogFileSaver _logSaver = new LogFileSaver(StandardLocations.Instance);
        private readonly DispatcherTimer _titleTimer = new DispatcherTimer(DispatcherPriority.Background);

        public MainWindow()
        {
            InitializeComponent();

            _logSaver.BeginAutoSave();

            ProgramOptionHolder.Instance.ProgramOptions = new ProgramOptions();
            UserMessageSystemHolder.Instance.CurrentUserMessageSystem = new DesktopUserMessageSystem();

            _titleTimer.Interval = TimeSpan.FromMilliseconds(300);
            _titleTimer.Start();
            _titleTimer.Tick += UpdateTimer_Tick;

            Sidebar_Main.AssociatedTaskView = TaskView_Main;
        }

        private async Task FinishLoading()
        {
            var taskView = TaskView_Main;

            if (taskView != null)
            {
                taskView.ProgramDisplayCreator = new ProgramDisplayCreator();
                taskView.CreateNewTask(true);
            }
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var splash = Splash_Overlay as ISplashScreen;
            if (splash != null)
            {
                _ = Activate();

                if (await splash.RunTaskAsync(new MainWindowLoadingTask()))
                {
                    await FinishLoading();
                    return;
                }
            }

            Close();
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
