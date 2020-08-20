using System.Windows;

namespace InDoOut_UI_Common.Windows
{
    public partial class NetworkConnectWindow : Window
    {
        public NetworkConnectWindow()
        {
            InitializeComponent();
        }

        private void Button_Connect_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void Button_NewConnection_Click(object sender, RoutedEventArgs e)
        {
            var newConnectionWindow = new NewNetworkConnectionWindow()
            {
                Owner = this
            };

            if ((newConnectionWindow.ShowDialog() ?? false) && newConnectionWindow.Valid)
            {
                var address = newConnectionWindow.Address;
                var port = newConnectionWindow.Port;


            }
        }
    }
}
