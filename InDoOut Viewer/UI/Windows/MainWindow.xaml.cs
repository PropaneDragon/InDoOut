using InDoOut_UI_Common.Windows;
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
            await Task.Delay(TimeSpan.FromMilliseconds(1));

            var networkConnectionWindow = new NewNetworkConnectionWindow();
            if (networkConnectionWindow.ShowDialog() ?? false)
            {

            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {

        }
    }
}
