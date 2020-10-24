using System.Threading;
using System.Windows;

namespace InDoOut_UI_Common.Windows
{
    public partial class TaskProgressWindow : Window
    {
        private readonly CancellationTokenSource _cancellationTokenSource = null;

        private bool _canClose = false;

        public string Caption { get => Text_Title.Text; set => Text_Title.Text = value; }
        public string Message { get => Text_Description.Text; set => SetMessageText(value); }

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

        private void SetMessageText(string text)
        {
            Text_Description.Text = text;
            Text_Description.Visibility = string.IsNullOrEmpty(text) ? Visibility.Collapsed : Visibility.Visible;
        }

        private void CancelAndClose()
        {
            if ((!_cancellationTokenSource?.IsCancellationRequested) ?? false)
            {
                Message = "Cancelling...";

                _cancellationTokenSource.Cancel();
            }

            _canClose = true;

            Close();
        }

        private void Button_Cancel_Click(object sender, RoutedEventArgs e) => CancelAndClose();

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = !_canClose;

            if (!_canClose)
            {
                CancelAndClose();
            }
        }
    }
}
