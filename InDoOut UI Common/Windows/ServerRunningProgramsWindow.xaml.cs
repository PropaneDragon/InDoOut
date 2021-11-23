using InDoOut_Networking.Client;
using InDoOut_Networking.Client.Commands;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace InDoOut_UI_Common.Windows
{
    public partial class ServerRunningProgramsWindow : Window
    {
        private readonly DispatcherTimer _loadTimer = new(DispatcherPriority.Normal) { Interval = TimeSpan.FromMilliseconds(100) };

        public IClient Client { get; set; } = null;

        public ServerRunningProgramsWindow()
        {
            InitializeComponent();

            _loadTimer.Tick += LoadTimer_Tick;
        }

        public ServerRunningProgramsWindow(IClient client) : this()
        {
            Client = client;
        }

        private async Task Refresh()
        {
            if (Client != null)
            {
                var cancellationToken = new CancellationTokenSource(TimeSpan.FromSeconds(5));
                var requestProgramsCommand = new RequestProgramsClientCommand(Client);
                var programStatusCommand = new GetProgramStatusClientCommand(Client);
                var progressWindow = new TaskProgressWindow("Loading programs", "The programs are being loaded from the server. Please wait...", cancellationToken) { Owner = this };

                progressWindow.TaskStarted();

                var programIds = await requestProgramsCommand.RequestAvailableProgramsAsync(new CancellationTokenSource(TimeSpan.FromSeconds(2)).Token);
                if (programIds != null)
                {
                    var tasks = programIds.Select(async programGuid => await programStatusCommand.GetProgramStatusAsync(programGuid, new CancellationTokenSource(TimeSpan.FromSeconds(5)).Token));
                    var programStatuses = await Task.WhenAll(tasks);

                    List_Programs.ItemsSource = programStatuses;
                }

                progressWindow.TaskFinished();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e) => _loadTimer.Start();

        private async void LoadTimer_Tick(object sender, EventArgs e)
        {
            _loadTimer.Stop();

            await Refresh();
        }

        private async void Button_Refresh_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button senderButton)
            {
                senderButton.IsEnabled = false;

                await Refresh();

                senderButton.IsEnabled = true;
            }
        }
    }
}