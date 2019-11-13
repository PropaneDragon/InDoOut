using InDoOut_Desktop.Options;
using InDoOut_Desktop.UI.Controls.Options;
using InDoOut_Core.Options;
using System.Collections.Generic;
using System.Windows;
using InDoOut_Plugins.Loaders;
using System;

namespace InDoOut_Desktop.UI.Windows
{
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();

            AddProgramOptions();
            AddPluginOptions();
        }

        private void AddProgramOptions()
        {
            AddOptions("Program options", ProgramSettings.Instance.OptionHolder.Options);
        }

        private void AddPluginOptions()
        {
            foreach (var pluginContainer in LoadedPlugins.Instance.Plugins)
            {
                var plugin = pluginContainer?.Plugin;
                if (plugin != null)
                {
                    var pluginName = plugin?.SafeName;
                    var pluginOptions = plugin?.OptionHolder;

                    if (pluginOptions != null && !string.IsNullOrEmpty(pluginName))
                    {
                        AddOptions(pluginName, pluginOptions.Options);
                    }
                }
            }
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

            _ = await OptionsSaveLoad.Instance.SaveAllOptionsAsync(this);

            Button_Apply.IsEnabled = true;
        }

        private void Button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
