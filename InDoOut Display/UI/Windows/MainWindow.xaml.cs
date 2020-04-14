using InDoOut_Display.Loading;
using InDoOut_Display.Programs;
using InDoOut_Executable_Core.Location;
using InDoOut_Executable_Core.Logging;
using InDoOut_UI_Common.Controls.Screens;
using System.Threading.Tasks;
using System.Windows;

namespace InDoOut_Display.UI.Windows
{
    public partial class MainWindow : Window
    {
        private readonly LogFileSaver _logSaver = new LogFileSaver(StandardLocations.Instance);

        public MainWindow()
        {
            InitializeComponent();

            _logSaver.BeginAutoSave();

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
    }
}
