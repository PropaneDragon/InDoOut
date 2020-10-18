using InDoOut_UI_Common.InterfaceElements;
using System.Windows.Controls;

namespace InDoOut_Viewer.UI.Controls.Sidebar
{
    public partial class Sidebar : UserControl
    {
        public ITaskView AssociatedTaskView { get; set; } = null;

        public Sidebar()
        {
            InitializeComponent();
        }

        private void Button_Settings_Click(object sender, System.Windows.RoutedEventArgs e)
        {

        }

        private void Button_Upload_Click(object sender, System.Windows.RoutedEventArgs e)
        {

        }
    }
}
