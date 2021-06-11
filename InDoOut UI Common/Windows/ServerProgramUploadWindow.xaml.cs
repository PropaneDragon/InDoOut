using InDoOut_Core.Functions;
using InDoOut_Executable_Core.Messaging;
using InDoOut_Networking.Client;
using InDoOut_Networking.Client.Commands;
using InDoOut_Plugins.Loaders;
using InDoOut_UI_Common.SaveLoad;
using System;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace InDoOut_UI_Common.Windows
{
    public partial class ServerProgramUploadWindow : Window
    {
        private string _programPath = null;

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

        private string GetFilePathFromDialog() => CommonProgramSaveLoad.Instance.LoadProgramPath(this);

        private string LoadProgramData(string path)
        {
            if (File.Exists(path))
            {
                try
                {
                    return File.ReadAllText(path);
                }
                catch
                {
                }
            }

            return null;
        }

        private void Button_Select_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button senderButton)
            {
                _programPath = GetFilePathFromDialog();

                senderButton.IsEnabled = true;
            }
        }

        private async void Button_Upload_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button senderButton)
            {
                senderButton.IsEnabled = false;

                if (!string.IsNullOrEmpty(_programPath))
                {
                    var programData = LoadProgramData(_programPath);

                    if (!string.IsNullOrEmpty(programData) && Client != null && FunctionBuilder != null)
                    {
                        var uploadCommand = new UploadProgramClientCommand(Client, LoadedPlugins.Instance, FunctionBuilder);
                        var cancellationToken = new CancellationTokenSource(TimeSpan.FromSeconds(10));
                        var programUploadingProgressWindow = new TaskProgressWindow("Uploading program", "The program is being uploaded. Please wait...", cancellationToken) { Owner = this };

                        programUploadingProgressWindow.TaskStarted();

                        var result = await uploadCommand.SendProgramAsync(programData, cancellationToken.Token);

                        programUploadingProgressWindow.TaskFinished();

                        if (result.Success)
                        {
                            UserMessageSystemHolder.Instance.CurrentUserMessageSystem.ShowInformation("Program uploaded", "The program was uploaded successfully");

                            DialogResult = true;

                            Close();
                        }
                        else
                        {
                            UserMessageSystemHolder.Instance.CurrentUserMessageSystem.ShowError("Program failed to upload", "There was a problem uploading the program to the server. Please try again later.", result.Message);
                        }
                    }
                    else
                    {
                        UserMessageSystemHolder.Instance.CurrentUserMessageSystem.ShowError("Can't load program to upload", "There appears to be a problem loading the program to upload.");
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
