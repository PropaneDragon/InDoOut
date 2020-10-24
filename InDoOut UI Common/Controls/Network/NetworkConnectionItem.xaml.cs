using InDoOut_UI_Common.Events;
using System;
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

        public event EventHandler<NetworkConnectionEventArgs> OnConnectButtonClicked;
        public event EventHandler OnRemoveButtonClicked;
        public event EventHandler OnEditButtonClicked;

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

        private void Button_Connect_Click(object sender, RoutedEventArgs e) => OnConnectButtonClicked?.Invoke(this, new NetworkConnectionEventArgs(Address, Port));

        private void Button_Remove_Click(object sender, RoutedEventArgs e) => OnRemoveButtonClicked?.Invoke(this, new EventArgs());

        private void Button_Edit_Click(object sender, RoutedEventArgs e) => OnEditButtonClicked?.Invoke(this, new EventArgs());
    }
}
