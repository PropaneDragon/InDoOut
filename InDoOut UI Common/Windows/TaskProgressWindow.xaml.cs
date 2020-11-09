using InDoOut_Executable_Core.Messaging;
using System.Threading;
using System.Windows;

namespace InDoOut_UI_Common.Windows
{
    public partial class TaskProgressWindow : Window
    {
        private readonly CancellationTokenSource _cancellationTokenSource = null;

        private bool _canClose = false;

        public string Caption { get => Header_Main.Title; set => Header_Main.Title = value; }
        public string Message { get => Header_Main.Subtitle; set => Header_Main.Subtitle = value; }

        private TaskProgressWindow()
        {
            InitializeComponent();

            Message = "";
            Caption = "";
        }

        public TaskProgressWindow(string caption, string message = "", CancellationTokenSource cancellationTokenSource = null) : this()
        {
            Caption = caption;
            Message = message;

            _cancellationTokenSource = cancellationTokenSource;
        }

        public TaskProgressWindow(string caption, CancellationTokenSource cancellationTokenSource, string message = "") : this(caption, message, cancellationTokenSource)
        {
        }

        public void TaskStarted() => Show();

        public void TaskFinished()
        {
            _canClose = true;

            Close();
        }

        private void TryCancel()
        {
            if (_cancellationTokenSource != null)
            {
                if (!_cancellationTokenSource.IsCancellationRequested)
                {
                    Message = "Cancelling...";

                    _cancellationTokenSource?.Cancel();
                }
                else
                {
                    UserMessageSystemHolder.Instance.CurrentUserMessageSystem.ShowError("Can't cancel", "A cancellation request is currently ongoing.");
                }
            }
            else
            {
                UserMessageSystemHolder.Instance.CurrentUserMessageSystem.ShowError("Can't cancel", "This task doesn't appear to be able to be cancelled.");
            }
        }

        private void Button_Cancel_Click(object sender, RoutedEventArgs e) => TryCancel();

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = !_canClose;

            if (!_canClose)
            {
                TryCancel();
            }
        }
    }
}
