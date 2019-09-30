﻿using InDoOut_Desktop.Loading;
using InDoOut_Desktop.UI.Interfaces;
using InDoOut_Desktop.UI.Threading;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace InDoOut_Desktop.UI.Windows
{
    public partial class MainWindow : Window
    {
        private static readonly bool OLD_SPLASH = false;

        private DispatcherTimer _titleTimer = new DispatcherTimer(DispatcherPriority.Background);

        public MainWindow()
        {
            InitializeComponent();

            UIThread.Instance.SetCurrentThreadAsUIThread();

            _titleTimer.Interval = TimeSpan.FromMilliseconds(300);
            _titleTimer.Start();
            _titleTimer.Tick += UpdateTimer_Tick;
        }

        private async Task FinishLoading()
        {
            var sidebar = Sidebar_Main;
            var taskView = TaskView_Main;

            if (taskView != null)
            {
                taskView.Sidebar = sidebar;
                taskView.CreateNewTask();
            }

            if (sidebar != null)
            {
                sidebar.TaskView = taskView;
            }
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var splash = OLD_SPLASH ? new SplashWindow() { Owner = this } : (ISplashScreen)Splash_Overlay;
            if (splash != null)
            {
                if (await splash.RunTaskAsync(new MainWindowLoadingTask()))
                {
                    await FinishLoading();
                    return;
                }
            }

            Close();
        }

        private void UpdateTimer_Tick(object sender, EventArgs e)
        {
            var programName = "No program";
            var program = TaskView_Main?.CurrentBlockView?.AssociatedProgram;

            if (program != null)
            {
                programName = string.IsNullOrEmpty(program.Name) ? "Untitled" : program.Name;
            }

            Title = $"{programName} > ido";
        }
    }
}
