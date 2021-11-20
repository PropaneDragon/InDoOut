using InDoOut_Core.Functions;
using InDoOut_Core.Logging;
using InDoOut_Executable_Core.Messaging;
using InDoOut_Networking.Client.Commands;
using InDoOut_Networking.Entities;
using InDoOut_UI_Common.InterfaceElements;
using InDoOut_UI_Common.Windows;
using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;

namespace InDoOut_Viewer.UI.Controls.Sidebar
{
    public partial class Sidebar : UserControl
    {
        private readonly DispatcherTimer _updateTimer = new DispatcherTimer(DispatcherPriority.Normal);

        public ITaskView AssociatedTaskView { get; set; } = null;

        public Sidebar()
        {
            InitializeComponent();

            _updateTimer.Interval = TimeSpan.FromMilliseconds(500);
            _updateTimer.Start();
            _updateTimer.Tick += UpdateTimer_Tick;
        }

        private void UpdateConnectionButtons()
        {
            foreach (var child in Grid_ConnectionButtons.Children)
            {
                if (child is ButtonBase button)
                {
                    button.Visibility = Visibility.Collapsed;
                }
            }

            if (AssociatedTaskView?.CurrentProgramDisplay?.AssociatedProgram is INetworkedProgram program && program.Connected)
            {
                Button_DisconnectFromRemote.Visibility = Visibility.Visible;
            }
            else
            {
                Button_ConnectToRemote.Visibility = Visibility.Visible;
            }
        }

        private void UpdatePlayStopButtons()
        {
            var program = AssociatedTaskView?.CurrentProgramDisplay?.AssociatedProgram;
            var programRunning = program?.Running ?? false;
            var programStopping = program?.Stopping ?? false;

            Button_RunProgram.Visibility = !programStopping && !programRunning ? Visibility.Visible : Visibility.Hidden;
            Button_StopProgram.Visibility = !programStopping && programRunning ? Visibility.Visible : Visibility.Hidden;
            Button_ProgramStopping.Visibility = programStopping ? Visibility.Visible : Visibility.Hidden;
        }

        private void UpdateConnectionOnlyButtons()
        {
            var connected = AssociatedTaskView?.CurrentProgramDisplay?.AssociatedProgram is INetworkedProgram program && program.Connected;

            Button_ViewProgram.IsEnabled = connected;
            Button_Upload.IsEnabled = connected;
            Grid_ProgramControlButtons.IsEnabled = connected;
        }

        private void UpdateTimer_Tick(object sender, EventArgs e)
        {
            UpdateConnectionButtons();
            UpdatePlayStopButtons();
            UpdateConnectionOnlyButtons();
        }

        private void Button_Settings_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Upload_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button senderButton && AssociatedTaskView?.CurrentProgramDisplay?.AssociatedProgram is NetworkedProgram program)
            {
                senderButton.IsEnabled = false;

                if (program.Connected)
                {
                    var programUploader = new ServerProgramUploadWindow(program.AssociatedClient, new FunctionBuilder());
                    _ = programUploader.ShowDialog();
                }
                else
                {
                    UserMessageSystemHolder.Instance.CurrentUserMessageSystem.ShowError("Please connect to a server first", "A program can only be uploaded if connected to a server.");
                }

                senderButton.IsEnabled = false;
            }
        }

        private void Button_ConnectToRemote_Click(object sender, RoutedEventArgs e)
        {
            if (AssociatedTaskView?.CurrentProgramDisplay != null)
            {
                var networkConnectWindow = new NetworkConnectWindow() { Owner = Window.GetWindow(this) };

                var client = networkConnectWindow.GetClient();
                if (client != null)
                {
                    AssociatedTaskView.CurrentProgramDisplay.AssociatedProgram = new NetworkedProgram(client);
                }
            }
        }

        private async void Button_DisconnectFromRemote_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button senderButton && AssociatedTaskView?.CurrentProgramDisplay?.AssociatedProgram is NetworkedProgram program)
            {
                senderButton.IsEnabled = false;

                try
                {
                    _ = await program.Disconnect();
                }
                catch { }

                senderButton.IsEnabled = true;
            }
        }

        private async void Button_ViewProgram_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button senderButton && AssociatedTaskView?.CurrentProgramDisplay?.AssociatedProgram is NetworkedProgram program)
            {
                senderButton.IsEnabled = false;

                if (program.Connected)
                {
                    var programSelectionWindow = new ServerProgramSelectionWindow(program.AssociatedClient) { Owner = Window.GetWindow(this) };
                    if (programSelectionWindow.ShowDialog() ?? false)
                    {
                        var selectedProgram = programSelectionWindow.SelectedProgramName;

                        if (!string.IsNullOrEmpty(selectedProgram))
                        {
                            var programDownloader = new DownloadProgramClientCommand(program.AssociatedClient);
                            var progressWindow = new TaskProgressWindow("Downloading program") { Owner = Window.GetWindow(this) };

                            progressWindow.TaskStarted();
                            var hasDownloaded = await programDownloader.RequestProgramAsync(selectedProgram, program, new CancellationTokenSource(TimeSpan.FromSeconds(5)).Token);
                            progressWindow.TaskFinished();

                            if (hasDownloaded)
                            {
                                AssociatedTaskView.CurrentProgramDisplay.AssociatedProgram = program;
                            }
                            else
                            {
                                UserMessageSystemHolder.Instance.CurrentUserMessageSystem.ShowError("Program couldn't be downloaded", "The chosen program couldn't be downloaded due to an error.");
                            }
                        }
                    }
                }
                else
                {
                    UserMessageSystemHolder.Instance.CurrentUserMessageSystem.ShowError("Please connect to a server first", "A program can only be selected if connected to a server.");
                }

                senderButton.IsEnabled = true;
            }
        }

        private void Button_RunningPrograms_Click(object sender, RoutedEventArgs e)
        {
            if (AssociatedTaskView?.CurrentProgramDisplay != null)
            {
                var networkConnectWindow = new NetworkConnectWindow() { Owner = Window.GetWindow(this) };

                var client = networkConnectWindow.GetClient();
                if (client != null)
                {
                    var runningProgramsWindow = new ServerRunningProgramsWindow(client) { Owner = Window.GetWindow(this) };
                    _ = runningProgramsWindow.ShowDialog();
                }
            }
        }

        private void Button_RunProgram_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button senderButton && AssociatedTaskView?.CurrentProgramDisplay?.AssociatedProgram is NetworkedProgram program)
            {
                senderButton.IsEnabled = false;

                if (program.Connected)
                {
                    program.Trigger(null);
                }

                senderButton.IsEnabled = true;
            }
        }

        private void Button_StopProgram_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button senderButton && AssociatedTaskView?.CurrentProgramDisplay?.AssociatedProgram is NetworkedProgram program)
            {
                senderButton.IsEnabled = false;

                if (program.Connected)
                {
                    program.Stop();
                }

                senderButton.IsEnabled = true;
            }
        }

        private void Button_ProgramStopping_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_TaskViewer_Click(object sender, RoutedEventArgs e)
        {
            if (AssociatedTaskView != null)
            {
                AssociatedTaskView?.ShowTasks();
            } 
        }

        private void Button_SwitchMode_Click(object sender, RoutedEventArgs e)
        {
            Log.Instance.Header("Switch button clicked");

            if (AssociatedTaskView?.CurrentProgramDisplay != null)
            {
                AssociatedTaskView.CurrentProgramDisplay.CurrentViewMode = AssociatedTaskView.CurrentProgramDisplay.CurrentViewMode == ProgramViewMode.IO ? ProgramViewMode.Variables : ProgramViewMode.IO;
            }
        }
    }
}
