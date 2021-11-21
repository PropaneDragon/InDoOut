using InDoOut_UI_Common.Events;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace InDoOut_UI_Common.Controls.Network
{
    public partial class NetworkConnectionItem : UserControl
    {
        private int _port = 0;
        private string _address = null;

        public int Port { get => _port; set => UpdatePort(value); }
        public string Address { get => _address; set => UpdateAddress(value); }
        public string FriendlyName { get => Text_Name.Text; set => Text_Name.Text = value ?? "Unnamed"; }

        public event EventHandler<NetworkConnectionEventArgs> OnConnectButtonClicked;
        public event EventHandler OnRemoveButtonClicked;
        public event EventHandler OnEditButtonClicked;

        public NetworkConnectionItem()
        {
            InitializeComponent();

            Border_HiddenContent.Opacity = 0d;
        }

        public NetworkConnectionItem(string name, string address, int port) : this()
        {
            FriendlyName = name;
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

        private void UserControl_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var fadeInAnimation = new DoubleAnimation(1, TimeSpan.FromMilliseconds(300)) { EasingFunction = new CircleEase() { EasingMode = EasingMode.EaseInOut } };

            Border_HiddenContent.BeginAnimation(OpacityProperty, fadeInAnimation);
        }

        private void UserControl_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var fadeOutAnimation = new DoubleAnimation(0, TimeSpan.FromMilliseconds(300)) { EasingFunction = new CircleEase() { EasingMode = EasingMode.EaseInOut } };

            Border_HiddenContent.BeginAnimation(OpacityProperty, fadeOutAnimation);
        }

        private void UpdateVisibleAddress() => Text_Address.Text = $"{_address ?? "unknown"}:{_port}";

        private void Button_Connect_Click(object sender, RoutedEventArgs e) => OnConnectButtonClicked?.Invoke(this, new NetworkConnectionEventArgs(Address, Port));

        private void Button_Remove_Click(object sender, RoutedEventArgs e) => OnRemoveButtonClicked?.Invoke(this, new EventArgs());

        private void Button_Edit_Click(object sender, RoutedEventArgs e) => OnEditButtonClicked?.Invoke(this, new EventArgs());
    }
}
