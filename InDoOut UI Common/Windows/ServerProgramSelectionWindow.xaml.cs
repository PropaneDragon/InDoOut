using InDoOut_Executable_Core.Messaging;
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
    public partial class ServerProgramSelectionWindow : Window
    {
        private readonly DispatcherTimer _loadTimer = new DispatcherTimer(DispatcherPriority.Normal) { Interval = TimeSpan.FromMilliseconds(100) };

        private bool _populatingPrograms = false;

        public string SelectedProgramName { get; private set; } = null;

        public IClient Client { get; set; } = null;

        public ServerProgramSelectionWindow()
        {
            InitializeComponent();
            UpdateCommonButtonVisibility();

            _loadTimer.Tick += LoadTimer_Tick;
        }

        public ServerProgramSelectionWindow(IClient client) : this()
        {
            Client = client;
        }

        public async Task<bool> PopulatePrograms()
        {
            var populated = false;

            _populatingPrograms = true;

            UpdateCommonButtonVisibility();

            if (Client != null)
            {
                var cancellationToken = new CancellationTokenSource(TimeSpan.FromSeconds(5));
                var requestProgramsCommand = new RequestProgramsClientCommand(Client);
                var programStatusCommand = new GetProgramStatusClientCommand(Client);
                var progressWindow = new TaskProgressWindow("Loading programs", "The programs are being loaded from the server. Please wait...", cancellationToken) { Owner = this };

                progressWindow.TaskStarted();

                try
                {
                    var availableProgramGuids = await requestProgramsCommand.RequestAvailableProgramsAsync(cancellationToken.Token);
                    if (availableProgramGuids != null)
                    {
                        if (availableProgramGuids.Count() > 0)
                        {
                            var tasks = availableProgramGuids.Select(async programGuid => await programStatusCommand.GetProgramStatusAsync(programGuid, new CancellationTokenSource(TimeSpan.FromSeconds(5)).Token));
                            var programStatuses = await Task.WhenAll(tasks);
                            var programNames = programStatuses.Select(programStatus => programStatus?.Name).Where(programName => !string.IsNullOrEmpty(programName));

                            List_Programs.ItemsSource = programNames;
                        }
                        else
                        {
                            UserMessageSystemHolder.Instance.CurrentUserMessageSystem.ShowInformation("No programs available", "The server doesn't appear to have any programs available.");
                        }

                        populated = true;
                    }
                    else
                    {
                        UserMessageSystemHolder.Instance.CurrentUserMessageSystem.ShowError("Couldn't get list of programs", "There was a problem receiving data from the server.");
                    }
                }
                catch (TaskCanceledException)
                {
                    UserMessageSystemHolder.Instance.CurrentUserMessageSystem.ShowError("Couldn't get list of programs", "The process was cancelled or timed out.");
                }

                progressWindow.TaskFinished();
            }
            else
            {
                UserMessageSystemHolder.Instance.CurrentUserMessageSystem.ShowError("Couldn't get list of programs", "There was a problem loading the programs from the server as there is no active connection.");
            }

            _populatingPrograms = false;

            UpdateCommonButtonVisibility();

            return populated;
        }

        private void UpdateCommonButtonVisibility()
        {
            Button_Refresh.IsEnabled = !_populatingPrograms;
            Button_Cancel.IsEnabled = !_populatingPrograms;
            Button_Select.IsEnabled = !_populatingPrograms && List_Programs.SelectedItem is string programName && !string.IsNullOrEmpty(programName);
        }

        private async void Button_Refresh_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button senderButton)
            {
                senderButton.IsEnabled = false;

                _ = await PopulatePrograms();

                senderButton.IsEnabled = true;
            }
        }

        private void List_Programs_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e) => UpdateCommonButtonVisibility();

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _loadTimer.Start();

            UpdateCommonButtonVisibility();
        }

        private async void LoadTimer_Tick(object sender, EventArgs e)
        {
            _loadTimer.Stop();

            _ = await PopulatePrograms();
        }

        private void Button_Cancel_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Select_Click(object sender, RoutedEventArgs e)
        {
            SelectedProgramName = List_Programs.SelectedItem as string;

            try
            {
                DialogResult = true;
            }
            catch
            {
                Close();
            }
        }
    }
}
