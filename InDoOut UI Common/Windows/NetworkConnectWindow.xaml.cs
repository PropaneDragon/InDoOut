using InDoOut_Executable_Core.Messaging;
using InDoOut_Executable_Core.Networking;
using InDoOut_Executable_Core.Networking.Entities;
using InDoOut_UI_Common.Controls.Network;
using InDoOut_UI_Common.InterfaceElements;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;

namespace InDoOut_UI_Common.Windows
{
    public partial class NetworkConnectWindow : Window
    {
        public ICommonProgramDisplay ProgramDisplay { get; set; } = null;

        public NetworkConnectWindow()
        {
            InitializeComponent();
        }

        public NetworkConnectWindow(ICommonProgramDisplay display) : this()
        {
            ProgramDisplay = display;
        }

        public async Task<bool> AcceptConnection(string address, int port)
        {
            var connected = false;

            if (!string.IsNullOrEmpty(address) && port > 0)
            {
                var client = new Client(); //Todo
                var program = new NetworkedProgram(client);
                var connectContent = Button_Connect.Content;

                Button_Cancel.IsEnabled = false;
                Button_Connect.IsEnabled = false;
                Button_Connect.Content = "Connecting...";

                try
                {
                    var ipAddresses = await Dns.GetHostAddressesAsync(address);
                    if (ipAddresses.Length > 0)
                    {
                        connected = await client.Connect(ipAddresses.First(), port);

                        if (connected && ProgramDisplay != null)
                        {
                            ProgramDisplay.AssociatedProgram = program;
                        }
                    }
                    else
                    {
                        UserMessageSystemHolder.Instance.CurrentUserMessageSystem.ShowError("Failed to connect", "The IP address could't be resolved from the given address.");
                    }
                }
                catch (Exception ex)
                {
                    UserMessageSystemHolder.Instance.CurrentUserMessageSystem.ShowError("Failed to connect", $"Failed to connect to the given address due to an error.", ex.Message);
                }

                Button_Cancel.IsEnabled = true;
                Button_Connect.IsEnabled = true;
                Button_Connect.Content = connectContent;
            }

            return connected;
        }

        private void CreateConnectionItem(string address, int port)
        {
            if (!string.IsNullOrEmpty(address))
            {
                var networkConnectionItem = new NetworkConnectionItem(address, port);

                _ = Wrap_Connections.Children.Add(networkConnectionItem);
            }
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

                CreateConnectionItem(address, port);
            }
        }
    }
}
