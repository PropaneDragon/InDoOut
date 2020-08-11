using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace InDoOut_UI_Common.Windows
{
    public partial class MessageBoxWindow : Window
    {
        private MessageBoxButton _buttons = MessageBoxButton.OK;
        private MessageBoxImage _image = MessageBoxImage.Information;

        private Dictionary<Button, MessageBoxResult> _buttonAssociations = null;

        public string Message { get => Text_Description.Text; set => Text_Description.Text = value; }

        public string Caption { get => Text_Title.Text; set => SetTitle(value); }

        public string Details { get => Text_Details.Text; set => SetDetails(value); }

        public MessageBoxButton Buttons { get => _buttons; set => SetMessageBoxButtons(value); }

        public MessageBoxResult Result { get; private set; } = MessageBoxResult.None;

        public MessageBoxImage Image { get => _image; set => SetMessageBoxImage(value); }

        private MessageBoxWindow()
        {
            InitializeComponent();

            _buttonAssociations = new Dictionary<Button, MessageBoxResult>
            {
                { Button_Cancel, MessageBoxResult.Cancel },
                { Button_No, MessageBoxResult.No },
                { Button_Ok, MessageBoxResult.OK },
                { Button_Yes, MessageBoxResult.Yes }
            };
        }

        public MessageBoxWindow(string message, string caption = "Message", string details = null, MessageBoxButton buttons = MessageBoxButton.OK, MessageBoxImage image = MessageBoxImage.Information) : this()
        {
            Message = message;
            Caption = caption;
            Details = details;
            Buttons = buttons;
            Image = image;
        }

        public static MessageBoxResult Show(string message, string caption = "Message", string details = null, MessageBoxButton buttons = MessageBoxButton.OK, MessageBoxImage image = MessageBoxImage.Information)
        {
            var apartmentState = Thread.CurrentThread.GetApartmentState();

            MessageBoxWindow messageBox = null;

            if (apartmentState != ApartmentState.STA)
            {
                var temporaryThread = new Thread(new ThreadStart(() => messageBox = CreateAndShow(message, caption, details, buttons, image)));
                temporaryThread.SetApartmentState(ApartmentState.STA);
                temporaryThread.Start();

                while (temporaryThread.IsAlive)
                {
                    Thread.Sleep(TimeSpan.FromMilliseconds(1));
                }
            }
            else
            {
                messageBox = CreateAndShow(message, caption, details, buttons, image);
            }            

            return messageBox?.Result ?? MessageBoxResult.None;
        }

        private static MessageBoxWindow CreateAndShow(string message, string caption = "Message", string details = null, MessageBoxButton buttons = MessageBoxButton.OK, MessageBoxImage image = MessageBoxImage.Information)
        {
            var messageBox = new MessageBoxWindow(message, caption, details, buttons, image);
            _ = messageBox.ShowDialog();

            return messageBox;
        }

        private void SetTitle(string title)
        {
            Text_Title.Text = title;
        }

        private void SetTitle(MessageBoxImage image)
        {
            switch (image)
            {
                case MessageBoxImage.Error:
                    Title = "Error";
                    break;
                case MessageBoxImage.Question:
                    Title = "Question";
                    break;
                case MessageBoxImage.Information:
                    Title = "Information";
                    break;
                case MessageBoxImage.Warning:
                    Title = "Warning";
                    break;
                default:
                    Title = "Message";
                    break;
            }
        }

        private void SetMessageBoxButtons(MessageBoxButton button)
        {
            _buttons = button;

            HideAllButtons();

            switch (button)
            {
                case MessageBoxButton.OK:
                    Button_Ok.Visibility = Visibility.Visible;
                    break;
                case MessageBoxButton.OKCancel:
                    Button_Cancel.Visibility = Visibility.Visible;
                    Button_Ok.Visibility = Visibility.Visible;
                    break;
                case MessageBoxButton.YesNo:
                    Button_Yes.Visibility = Visibility.Visible;
                    Button_No.Visibility = Visibility.Visible;
                    break;
                case MessageBoxButton.YesNoCancel:
                    Button_Yes.Visibility = Visibility.Visible;
                    Button_No.Visibility = Visibility.Visible;
                    Button_Cancel.Visibility = Visibility.Visible;
                    break;
            }
        }

        private void SetMessageBoxImage(MessageBoxImage image)
        {
            _image = image;

            HideAllImages();
            SetTitle(image);

            switch (image)
            {
                case MessageBoxImage.Error:
                    Icon_Critical.Visibility = Visibility.Visible;
                    break;
                case MessageBoxImage.Question:
                    Icon_Question.Visibility = Visibility.Visible;
                    break;
                case MessageBoxImage.Information:
                    Icon_Info.Visibility = Visibility.Visible;
                    break;
                case MessageBoxImage.Warning:
                    Icon_Warning.Visibility = Visibility.Visible;
                    break;
                default:
                    Icon_Info.Visibility = Visibility.Visible;
                    break;
            }
        }

        private void SetDetails(string details)
        {
            Text_Details.Text = details ?? "";
            Scroll_Details.Visibility = !string.IsNullOrEmpty(details) ? Visibility.Visible : Visibility.Collapsed;
        }

        private void HideAllButtons()
        {
            foreach (UIElement child in Dock_Buttons.Children)
            {
                if (child is Button button)
                {
                    button.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void HideAllImages()
        {
            foreach (UIElement child in Grid_Icons.Children)
            {
                if (child is TextBlock textBlock)
                {
                    textBlock.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void Button_Bottom_Click(object sender, RoutedEventArgs e)
        {
            Result = MessageBoxResult.None;

            if (sender != null && sender is Button button)
            {
                if (_buttonAssociations.ContainsKey(button))
                {
                    Result = _buttonAssociations[button];
                }
            }

            DialogResult = true;
            Close();
        }
    }
}
