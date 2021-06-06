using InDoOut_Core.Entities.Programs;
using InDoOut_Core.Functions;
using InDoOut_Executable_Core.Messaging;
using InDoOut_Networking.Client;
using InDoOut_Networking.Client.Commands;
using InDoOut_Plugins.Loaders;
using InDoOut_UI_Common.SaveLoad;
using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace InDoOut_UI_Common.Windows
{
    public partial class ServerProgramUploadWindow : Window
    {
        private IProgram _selectedProgram = null;

        public IClient Client { get; set; } = null;
        public IFunctionBuilder FunctionBuilder { get; set; } = null;

        public ServerProgramUploadWindow()
        {
            InitializeComponent();
        }

        public ServerProgramUploadWindow(IClient client, IFunctionBuilder functionBuilder) : this()
        {
            Client = client;
            FunctionBuilder = functionBuilder;
        }

        private async void Button_Select_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button senderButton)
            {
                senderButton.IsEnabled = false;

                var programLoadingProgressWindow = new TaskProgressWindow("Loading program", "The program is being loaded to upload. Please wait...") { Owner = this };
                programLoadingProgressWindow.TaskStarted();

                var program = await CommonProgramSaveLoad.Instance.LoadProgramDialogAsync(this);

                programLoadingProgressWindow.TaskFinished();

                _selectedProgram = program;

                senderButton.IsEnabled = true;
            }
        }

        private async void Button_Upload_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button senderButton)
            {
                senderButton.IsEnabled = false;

                if (_selectedProgram != null)
                {
                    if (Client != null && FunctionBuilder != null)
                    {
                        var uploadCommand = new UploadProgramClientCommand(Client, LoadedPlugins.Instance, FunctionBuilder);
                        var cancellationToken = new CancellationTokenSource(TimeSpan.FromSeconds(10));
                        var programUploadingProgressWindow = new TaskProgressWindow("Uploading program", "The program is being uploaded. Please wait...", cancellationToken) { Owner = this };

                        programUploadingProgressWindow.TaskStarted();

                        var result = await uploadCommand.SendProgramAsync(_selectedProgram, cancellationToken.Token);

                        programUploadingProgressWindow.TaskFinished();

                        if (result)
                        {
                            UserMessageSystemHolder.Instance.CurrentUserMessageSystem.ShowInformation("Program uploaded", "The program was uploaded successfully");

                            DialogResult = true;

                            Close();
                        }
                        else
                        {
                            UserMessageSystemHolder.Instance.CurrentUserMessageSystem.ShowError("Program failed to upload", "There was a problem uploading the program to the server. Please try again later.");
                        }
                    }
                    else
                    {
                        UserMessageSystemHolder.Instance.CurrentUserMessageSystem.ShowError("Can't create program to upload", "There appears to be a problem creating the program to upload.");
                    }
                }
                else
                {
                    UserMessageSystemHolder.Instance.CurrentUserMessageSystem.ShowError("Please select a program", "Please select a program to upload before uploading.");
                }

                senderButton.IsEnabled = true;
            }
        }

        private void Button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;

            Close();
        }
    }
}
