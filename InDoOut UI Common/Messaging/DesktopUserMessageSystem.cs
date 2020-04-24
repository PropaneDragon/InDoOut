using InDoOut_Executable_Core.Messaging;
using System.Windows;

namespace InDoOut_UI_Common.Messaging
{
    public class DesktopUserMessageSystem : AbstractUserMessageSystem
    {
        public override void ShowError(string title, string message)
        {
            _ = MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public override void ShowWarning(string title, string message)
        {
            _ = MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        public override void ShowInformation(string title, string message)
        {
            _ = MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public override UserResponse? ShowQuestion(string title, string message)
        {
            var result = MessageBox.Show(message, title, MessageBoxButton.YesNoCancel, MessageBoxImage.Question);

            return result switch
            {
                MessageBoxResult.Yes => UserResponse.Yes,
                MessageBoxResult.No => UserResponse.No,
                MessageBoxResult.Cancel => UserResponse.Cancel,

                _ => (UserResponse?)null
            };
        }
    }
}
