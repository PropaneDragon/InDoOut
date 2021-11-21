using InDoOut_Core.Options;
using InDoOut_Core.Options.Types;
using InDoOut_Executable_Core.Messaging;
using InDoOut_Executable_Core.Options;
using InDoOut_Networking.Client;
using InDoOut_UI_Common.Controls.Network;
using InDoOut_UI_Common.Events;
using InDoOut_UI_Common.SaveLoad;
using InDoOut_UI_Common.Storage.Options;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;

namespace InDoOut_UI_Common.Windows
{
    public partial class NetworkConnectWindow : Window
    {
        private static readonly HiddenJsonObjectOption _connectionItemsOption = new HiddenJsonObjectOption("NetworkConnectionItems");

        public IClient NetworkClient { get; private set; } = null;
        public IOptionHolder OptionHolder { get; set; } = null;
        public IOption<string> ConnectionItemsOption => _connectionItemsOption;

        public NetworkConnectWindow()
        {
            OptionHolder = ProgramOptionsHolder.Instance.ProgramOptions?.OptionHolder;

            InitializeComponent();
            LoadOptions();
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

        private void LoadOptions()
        {
            if (OptionHolder != null)
            {
                _ = OptionHolder.RegisterOption(_connectionItemsOption, true);
            }

            var connectionItems = _connectionItemsOption.ToObject<ConnectionOptionStorage>();
            if (connectionItems != null)
            {
                foreach (var connection in connectionItems.Connections)
                {
                    if (connection.Valid)
                    {
                        CreateConnectionItem(connection.Address, connection.Port);
                    }
                }
            }
        }

        private async Task<bool> SaveOptions()
        {
            var connectionStorage = new ConnectionOptionStorage();

            foreach (var child in Wrap_Connections.Children)
            {
                if (child is NetworkConnectionItem connectionItem)
                {
                    connectionStorage.Connections.Add(new ConnectionItem() { Address = connectionItem.Address, Port = connectionItem.Port });
                }
            }

            if (!_connectionItemsOption.FromObject(connectionStorage))
            {
                UserMessageSystemHolder.Instance.CurrentUserMessageSystem.ShowWarning("Couldn't save", $"The connection info couldn't be applied due to an internal error and won't be properly saved.");
            }

            return await CommonOptionsSaveLoad.Instance.SaveAllOptionsAsync();
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

        private async void Button_NewConnection_Click(object sender, RoutedEventArgs e)
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

            if (!await SaveOptions())
            {
                UserMessageSystemHolder.Instance.CurrentUserMessageSystem.ShowWarning("Couldn't save", $"The connection info couldn't be saved and may not be available next time the program starts.");
            }
        }
    }
}
