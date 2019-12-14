﻿using InDoOut_Display.Loading;
using InDoOut_Display.UI.Controls.ElementSelector;
using InDoOut_Executable_Core.Location;
using InDoOut_Executable_Core.Logging;
using System.Threading.Tasks;
using System.Windows;

namespace InDoOut_Display.UI.Windows
{
    public partial class MainWindow : Window
    {
        private readonly LogFileSaver _logSaver = new LogFileSaver(StandardLocations.Instance);

        public MainWindow()
        {
            InitializeComponent();

            _logSaver.BeginAutoSave();
        }

        private async Task FinishLoading()
        {
            if (Popup_Test.Content is ElementSelector ElementSelector_Overlay)
            {
                ElementSelector_Overlay.LoadElementsSlowly();
            }
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var loadingTask = new MainWindowLoadingTask();
            var loadedSuccessfully = await loadingTask.RunAsync();
            if (loadedSuccessfully)
            {
                await FinishLoading();
                return;
            }

            Close();
        }
    }
}
