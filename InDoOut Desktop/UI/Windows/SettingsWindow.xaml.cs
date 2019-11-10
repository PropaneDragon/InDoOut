using InDoOut_Desktop.Options;
using InDoOut_Desktop.UI.Controls.Options;
using InDoOut_Core.Options;
using System.Collections.Generic;
using System.Windows;
using System.Threading.Tasks;

namespace InDoOut_Desktop.UI.Windows
{
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();

            AddOptions("Program options", ProgramSettings.Instance.OptionHolder.Options);
        }

        private void AddOptions(string title, List<IOption> options)
        {
            if (options != null && options.Count > 0)
            {
                var display = new OptionsDisplay(title, options);

                AddOptions(display);
            }
        }

        private void AddOptions(OptionsDisplay display)
        {
            if (display != null)
            {
                _ = Stack_Options.Children.Add(display);
            }
        }

        private async void Button_Apply_Click(object sender, RoutedEventArgs e)
        {
            Button_Apply.IsEnabled = false;

            foreach (var potentialOption in Stack_Options.Children)
            {
                if (potentialOption is OptionsDisplay optionDisplay)
                {
                    optionDisplay.CommitChanges();
                }
            }

            _ = await OptionsSaveLoad.Instance.SaveProgramOptionsAsync(this);

            Button_Apply.IsEnabled = true;
        }

        private void Button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
