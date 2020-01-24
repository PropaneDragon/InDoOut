using InDoOut_Display.UI.Controls.Screens;
using InDoOut_Display.UI.Windows;
using System.Windows;
using System.Windows.Controls;

namespace InDoOut_Display.UI.Controls.Sidebar
{
    public partial class Sidebar : UserControl
    {
        public IScreenOverview AssociatedScreenOverview { get; set; } = null;

        public Sidebar()
        {
            InitializeComponent();
        }

        private void Button_Add_Display_Click(object sender, RoutedEventArgs e)
        {
            var window = Window.GetWindow(this);
            var elementWindow = new ElementSelectorWindow(AssociatedScreenOverview?.CurrentConnectionsScreen?.CurrentScreen)
            {
                Owner = Window.GetWindow(this)
            };

            if (window != null)
            {
                elementWindow.Width = window.Width - 200;
                elementWindow.Height = window.Height - 200;
            }

            elementWindow.Show();
        }

        private void Button_Add_Functions_Click(object sender, RoutedEventArgs e)
        {
            var window = Window.GetWindow(this);
            var elementWindow = new FunctionSelectorWindow(AssociatedScreenOverview?.CurrentConnectionsScreen)
            {
                Owner = Window.GetWindow(this),
                Width = 400
            };

            if (window != null)
            {
                var windowPosition = window.PointToScreen(new Point());

                elementWindow.WindowStartupLocation = WindowStartupLocation.Manual;
                elementWindow.Top = windowPosition.Y + 100;
                elementWindow.Left = windowPosition.X + 200;
                elementWindow.Height = window.Height - 200;
            }

            elementWindow.Show();
        }

        private void Button_SwitchMode_Click(object sender, RoutedEventArgs e)
        {
            var screen = AssociatedScreenOverview?.CurrentConnectionsScreen?.CurrentScreen;
            if (screen != null)
            {
                screen.Mode = screen.Mode == ScreenMode.Connections ? ScreenMode.Layout : ScreenMode.Connections;
            }
        }

        private void Button_New_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Open_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Save_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_SaveAs_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
