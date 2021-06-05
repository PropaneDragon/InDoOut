using InDoOut_Executable_Core.Messaging;
using InDoOut_Networking.Client.Commands;
using InDoOut_UI_Common.SaveLoad;
using System.Windows;

namespace InDoOut_UI_Common.Windows
{
    public partial class ServerProgramUploadWindow : Window
    {
        public ServerProgramUploadWindow()
        {
            InitializeComponent();
        }

        private async void Button_Select_Click(object sender, RoutedEventArgs e)
        {
            var program = await CommonProgramSaveLoad.Instance.LoadProgramDialogAsync(this);
            if (program != null)
            {
            }
            else
            {
                UserMessageSystemHolder.Instance.CurrentUserMessageSystem.ShowError("Invalid program", "The selected program appears to be invalid and can't be uploaded.")
            }
        }

        private void Button_Upload_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
