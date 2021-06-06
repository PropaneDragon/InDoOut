using InDoOut_Executable_Core.Messaging;
using InDoOut_UI_Common.Messaging;
using InDoOut_Viewer.Loading;
using InDoOut_Viewer.Programs;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace InDoOut_Viewer
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Application.Current.DispatcherUnhandledException += Application_DispatcherUnhandledException;
            UserMessageSystemHolder.Instance.CurrentUserMessageSystem = new DesktopUserMessageSystem();
            //ProgramOptionsHolder.Instance.ProgramOptions = new ProgramOptions();
        }

        private async Task FinishLoading()
        {
            await Task.CompletedTask;

            var taskView = TaskView_Main;
            var sidebar = Sidebar_Main;

            if (taskView != null)
            {
                taskView.ProgramDisplayCreator = new ProgramDisplayCreator();
                taskView.CreateNewTask(true);
            }

            if (sidebar != null)
            {
                sidebar.AssociatedTaskView = taskView;
            }
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var loadingTask = new MainWindowLoadingTask();

            if (await loadingTask.RunAsync())
            {
                await FinishLoading();
            }
            else
            {
                Close();
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {

        }

        private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e) => throw new NotImplementedException();
    }
}
