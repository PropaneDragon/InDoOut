using InDoOut_Core.Options;
using System.Windows.Controls;

namespace InDoOut_UI_Common.Controls.Options.Types
{
    public partial class CheckableOptionInterface : UserControl, ILinkedInterfaceOption
    {
        public CheckableOptionInterface()
        {
            InitializeComponent();
        }

        public bool UpdateFromOption(IOption option)
        {
            Text_Name.Text = option.Name ?? "Invalid option name";
            Text_Description.Text = option.Description ?? "";

            Button_Checked.IsChecked = option.ValueAs(false);

            UpdateButtonIcon();

            return true;
        }

        public bool UpdateOptionValue(IOption option) => option.ValueFrom(Button_Checked.IsChecked ?? false);

        private void UpdateButtonIcon() => Button_Checked.Content = (Button_Checked.IsChecked ?? false) ? "" : "";

        private void Button_Checked_Click(object sender, System.Windows.RoutedEventArgs e) => UpdateButtonIcon();

        private void UserControl_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Button_Checked.IsChecked = !Button_Checked.IsChecked;

            UpdateButtonIcon();
        }
    }
}
