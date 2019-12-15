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
    }
}
