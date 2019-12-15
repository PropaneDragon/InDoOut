using InDoOut_Display.UI.Windows;
using InDoOut_UI_Common.Controls.Popup;
using System.Windows;
using System.Windows.Controls;

namespace InDoOut_Display.UI.Controls.Sidebar
{
    public partial class Sidebar : UserControl
    {
        public Sidebar()
        {
            InitializeComponent();
        }

        private void Button_Add_Click(object sender, RoutedEventArgs e)
        {
            var elementWindow = new ElementSelectorWindow() { Owner = Window.GetWindow(this) };
            elementWindow.Show();
        }
    }
}
