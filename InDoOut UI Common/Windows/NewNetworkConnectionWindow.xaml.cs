using InDoOut_Executable_Core.Messaging;
using System;
using System.Windows;

namespace InDoOut_UI_Common.Windows
{
    public partial class NewNetworkConnectionWindow : Window
    {
        private int _port = -1;
        private string _address = null;

        public bool Valid => Port >= 0 && !string.IsNullOrWhiteSpace(Address);
        public int Port { get => _port; private set => SetPort(value); }
        public string Address { get => _address; private set => SetAddress(value); }

        public NewNetworkConnectionWindow()
        {
            InitializeComponent();
        }

        private int GetPort(string port) => !string.IsNullOrEmpty(port) ? (int.TryParse(port, out var parsedPort) ? parsedPort : throw new InvalidOperationException("The port must be numeric.")) : throw new InvalidOperationException("The port must have a value.");

        private void SetAddress(string address) => _address = !string.IsNullOrWhiteSpace(address) ? address : throw new InvalidOperationException("The address must have a value.");

        private void SetPort(int port) => _port = port >= 0 ? port : throw new InvalidOperationException("The port can't be a negative number.");

        private void Button_Add_Click(object sender, RoutedEventArgs e)
        {
            var address = TextBox_Address.Text;
            var port = TextBox_Port.Text;

            try
            {
                Address = address;
                Port = GetPort(port);

                DialogResult = true;
                Close();
            }
            catch (InvalidOperationException ex)
            {
                UserMessageSystemHolder.Instance.CurrentUserMessageSystem?.ShowError("Invalid details", $"The connection couldn't be added. {ex.Message}\n\nPlease try again.");
            }
        }

        private void Button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
