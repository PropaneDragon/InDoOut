using InDoOut_Desktop.Options;
using InDoOut_Desktop.UI.Controls.Options;
using InDoOut_Plugins.Options;
using System.Collections.Generic;
using System.Windows;

namespace InDoOut_Desktop.UI.Windows
{
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();

            AddOptions("Program options", new List<IOption>()
            {
                ProgramSettings.START_IN_BACKGROUND,
                ProgramSettings.START_WITH_COMPUTER
            });
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
                Stack_Options.Children.Add(display);
            }
        }
    }
}
