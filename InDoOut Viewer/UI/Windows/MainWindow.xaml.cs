using InDoOut_UI_Common.Windows;
using InDoOut_Viewer.Programs;
using System;
using System.Threading.Tasks;
using System.Windows;

namespace InDoOut_Viewer
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
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

        private void Window_Closed(object sender, EventArgs e)
        {

        }
    }
}
