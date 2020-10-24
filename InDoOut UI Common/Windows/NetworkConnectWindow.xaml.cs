using InDoOut_Executable_Core.Messaging;
using InDoOut_Executable_Core.Networking;
using InDoOut_UI_Common.Controls.Network;
using InDoOut_UI_Common.Events;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;

namespace InDoOut_UI_Common.Windows
{
    public partial class NetworkConnectWindow : Window
    {
        public IClient NetworkClient { get; private set; } = null;

        public NetworkConnectWindow()
        {
            InitializeComponent();
        }

        public async Task<IClient> Connect(string address, int port)
        {
            if (!string.IsNullOrEmpty(address) && port > 0)
            {
                Button_Cancel.IsEnabled = false;

                try
                {
                    var ipAddresses = await Dns.GetHostAddressesAsync(address);
                    if (ipAddresses.Length > 0)
                    {
                        var progressWindow = new TaskProgressWindow("Connecting", $"Connecting to {address}:{port}. Please wait...");
                        progressWindow.TaskStarted();

                        var client = new Client();
                        var connected = await client.Connect(ipAddresses.First(), port);

                        progressWindow.TaskFinished();

                        if (connected)
                        {
                            return client;
                        }
                        else
                        { 
                            UserMessageSystemHolder.Instance.CurrentUserMessageSystem.ShowError("Failed to connect", $"A connection couldn't be established to \"{address}\".");
                        }
                    }
                    else
                    {
                        UserMessageSystemHolder.Instance.CurrentUserMessageSystem.ShowError("Failed to connect", $"The IP address could't be resolved from the address \"{address}\".");
                    }
                }
                catch (Exception ex)
                {
                    UserMessageSystemHolder.Instance.CurrentUserMessageSystem.ShowError("Failed to connect", $"Failed to connect to the given address due to an error.", ex.Message);
                }

                Button_Cancel.IsEnabled = true;
            }

            return null;
        }

        public IClient GetClient() => ShowDialog() ?? false ? NetworkClient : null;

        private void CreateConnectionItem(string address, int port)
        {
            if (!string.IsNullOrEmpty(address))
            {
                var networkConnectionItem = new NetworkConnectionItem(address, port);

                AttachNetworkConnectionItemEvents(networkConnectionItem);

                _ = Wrap_Connections.Children.Add(networkConnectionItem);
            }
        }

        private void AttachNetworkConnectionItemEvents(NetworkConnectionItem connectionItem)
        {
            if (connectionItem != null)
            {
                connectionItem.OnConnectButtonClicked += ConnectionItem_OnConnectButtonClicked;
                connectionItem.OnRemoveButtonClicked += ConnectionItem_OnRemoveButtonClicked;
            }
        }

        private async void ConnectionItem_OnConnectButtonClicked(object sender, NetworkConnectionEventArgs e)
        {
            NetworkClient = await Connect(e.Address, e.Port);

            if (NetworkClient != null)
            {
                DialogResult = true;
                Close();
            }
        }

        private void ConnectionItem_OnRemoveButtonClicked(object sender, EventArgs e) => throw new NotImplementedException();

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

                CreateConnectionItem(address, port);
            }
        }
    }
}
