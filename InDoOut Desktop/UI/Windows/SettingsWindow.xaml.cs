using InDoOut_Desktop.Options;
using InDoOut_Plugins.Options;
using System.Collections.Generic;
using System.Windows;

namespace InDoOut_Desktop.UI.Windows
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();

            Options_Program.PopulateForOptions(new List<IOption>()
            {
                ProgramSettings.START_IN_BACKGROUND,
                ProgramSettings.START_WITH_COMPUTER
            });
        }
    }
}
