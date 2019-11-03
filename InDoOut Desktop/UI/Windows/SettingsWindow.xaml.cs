﻿using InDoOut_Desktop.Options;
using InDoOut_Desktop.UI.Controls.Options;
using InDoOut_Core.Options;
using System.Collections.Generic;
using System.Windows;

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
    }
}
