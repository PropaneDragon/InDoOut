using InDoOut_UI_Common.Windows;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace InDoOut_UI_Common.Controls.Network
{
    public partial class NetworkConnectionItem : UserControl
    {
        private int _port = 0;
        private string _address = null;

        public int Port { get => _port; set => UpdatePort(value); }
        public string Address { get => _address; set => UpdateAddress(value); }

        public NetworkConnectionItem()
        {
            InitializeComponent();
        }

        public NetworkConnectionItem(string address, int port) : this()
        {
            Address = address;
            Port = port;
        }

        private void UpdateAddress(string address)
        {
            _address = address;

            UpdateVisibleAddress();
        }

        private void UpdatePort(int port)
        {
            _port = port;

            UpdateVisibleAddress();
        }

        private void UpdateVisibleAddress() => Text_Address.Text = $"{_address ?? "unknown"}:{_port}";

        private async void Button_Connect_Click(object sender, RoutedEventArgs e)
        {
            var connectionWindow = Window.GetWindow(this) as NetworkConnectWindow; //Todo: Do this properly.

            _ = await (connectionWindow?.AcceptConnection(_address, _port) ?? Task.FromResult(false));
        }

        private void Button_Remove_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Edit_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
