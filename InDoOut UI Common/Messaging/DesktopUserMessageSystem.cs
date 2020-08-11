using InDoOut_Executable_Core.Messaging;
using InDoOut_UI_Common.Windows;
using System.Windows;

namespace InDoOut_UI_Common.Messaging
{
    public class DesktopUserMessageSystem : AbstractUserMessageSystem
    {
        public override void ShowError(string title, string message, string details = null)
        {
            _ = MessageBoxWindow.Show(message, title, details, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public override void ShowWarning(string title, string message, string details = null)
        {
            _ = MessageBoxWindow.Show(message, title, details, MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        public override void ShowInformation(string title, string message, string details = null)
        {
            _ = MessageBoxWindow.Show(message, title, details, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public override UserResponse? ShowQuestion(string title, string message)
        {
            var result = MessageBoxWindow.Show(message, title, null, MessageBoxButton.YesNoCancel, MessageBoxImage.Question);

            return result switch
            {
                MessageBoxResult.Yes => UserResponse.Yes,
                MessageBoxResult.No => UserResponse.No,
                MessageBoxResult.Cancel => UserResponse.Cancel,

                _ => null
            };
        }
    }
}
