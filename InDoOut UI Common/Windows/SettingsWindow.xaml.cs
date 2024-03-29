﻿using InDoOut_Core.Options;
using InDoOut_Executable_Core.Options;
using InDoOut_Executable_Core.Storage;
using InDoOut_Plugins.Loaders;
using InDoOut_UI_Common.Controls.Options;
using InDoOut_UI_Common.Extensions.Window;
using InDoOut_UI_Common.SaveLoad;
using System.Collections.Generic;
using System.Windows;

namespace InDoOut_UI_Common.Windows
{
    public partial class SettingsWindow : Window
    {
        private IOptionsStorer OptionsStorer { get; set; } = null;

        public SettingsWindow()
        {
            InitializeComponent();

            AddProgramOptions();
            AddPluginOptions();

            this.ResizeToOwner();
        }

        public SettingsWindow(IOptionsStorer optionsStorer) : this()
        {
            OptionsStorer = optionsStorer;
        }

        private void AddProgramOptions() => AddOptions("Program options", ProgramOptionsHolder.Instance.ProgramOptions?.OptionHolder?.Options);

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

        private void Resize()
        {
            if (Owner != null)
            {
                Height = Owner.Height - 100;
                Top = Owner.Top + 50;
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

            _ = await OptionsSaveLoad.Instance.SaveAllOptionsAsync(OptionsStorer);

            Button_Apply.IsEnabled = true;
        }

        private void Button_Cancel_Click(object sender, RoutedEventArgs e) => Close();

        private void Window_Loaded(object sender, RoutedEventArgs e) => this.ResizeToOwner();
    }
}
