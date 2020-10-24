using InDoOut_Networking.Entities;
using InDoOut_UI_Common.InterfaceElements;
using InDoOut_UI_Common.Windows;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;

namespace InDoOut_Viewer.UI.Controls.Sidebar
{
    public partial class Sidebar : UserControl
    {
        private readonly DispatcherTimer _updateTimer = new DispatcherTimer(DispatcherPriority.Normal);

        public ITaskView AssociatedTaskView { get; set; } = null;

        public Sidebar()
        {
            InitializeComponent();

            _updateTimer.Interval = TimeSpan.FromMilliseconds(500);
            _updateTimer.Start();
            _updateTimer.Tick += UpdateTimer_Tick;
        }

        private void UpdateConnectionButtons()
        {
            foreach (var child in Grid_ConnectionButtons.Children)
            {
                if (child is ButtonBase button)
                {
                    button.Visibility = Visibility.Collapsed;
                } 
            } 

            if (AssociatedTaskView?.CurrentProgramDisplay?.AssociatedProgram is INetworkedProgram program && program.Connected)
            {
                Button_DisconnectFromRemote.Visibility = Visibility.Visible;
            }
            else
            {
                Button_ConnectToRemote.Visibility = Visibility.Visible;
            } 
        }

        private void UpdatePlayStopButtons() { }

        private void UpdateTimer_Tick(object sender, EventArgs e)
        {
            UpdateConnectionButtons();
            UpdatePlayStopButtons();
        }

        private void Button_Settings_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Upload_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_ConnectToRemote_Click(object sender, RoutedEventArgs e)
        {
            if (AssociatedTaskView?.CurrentProgramDisplay != null)
            {
                var networkConnectWindow = new NetworkConnectWindow()
                {
                    Owner = Window.GetWindow(this)
                };

                var client = networkConnectWindow.GetClient();
                if (client != null)
                {
                    AssociatedTaskView.CurrentProgramDisplay.AssociatedProgram = new NetworkedProgram(client);
                }
            } 
        }

        private async void Button_DisconnectFromRemote_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button senderButton && AssociatedTaskView?.CurrentProgramDisplay?.AssociatedProgram is NetworkedProgram program)
            {
                senderButton.IsEnabled = false;

                try
                {
                    _ = await program.Disconnect();
                }
                catch { }

                senderButton.IsEnabled = true;
            }
        }
    }
}
