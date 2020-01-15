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

        private void Button_Add_Click(object sender, RoutedEventArgs e)
        {
            var elementWindow = new ElementSelectorWindow(AssociatedScreenOverview?.CurrentScreen) { Owner = Window.GetWindow(this) };
            elementWindow.Show();
        }

        private void Button_SwitchMode_Click(object sender, RoutedEventArgs e)
        {
            var screen = AssociatedScreenOverview?.CurrentScreen;
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
