using InDoOut_Core.Functions;
using InDoOut_Core.Logging;
using InDoOut_Display.Options;
using InDoOut_Display.UI.Windows;
using InDoOut_Display_Json_Storage;
using InDoOut_Executable_Core.Location;
using InDoOut_Executable_Core.Programs;
using InDoOut_Plugins.Loaders;
using InDoOut_UI_Common.InterfaceElements;
using InDoOut_UI_Common.SaveLoad;
using InDoOut_UI_Common.Windows;
using System;
using System.Windows;
using System.Windows.Controls;

namespace InDoOut_Display.UI.Controls.Sidebar
{
    public partial class Sidebar : UserControl
    {
        public ITaskView AssociatedTaskView { get; set; } = null;

        public Sidebar()
        {
            InitializeComponent();
        }

        private void Button_Add_Display_Click(object sender, RoutedEventArgs e)
        {
            var window = Window.GetWindow(this);
            var elementWindow = new ElementSelectorWindow(AssociatedTaskView?.CurrentProgramDisplay)
            {
                Owner = Window.GetWindow(this)
            };

            if (window != null)
            {
                elementWindow.Width = Math.Max(window.ActualWidth - 200, 200);
                elementWindow.Height = Math.Max(window.ActualHeight - 200, 200);
            }

            elementWindow.Show();
        }

        private void Button_Add_Functions_Click(object sender, RoutedEventArgs e)
        {
            var window = Window.GetWindow(this);
            var elementWindow = new FunctionSelectorWindow(AssociatedTaskView?.CurrentProgramDisplay)
            {
                Owner = Window.GetWindow(this),
                Width = 400
            };

            if (window != null)
            {
                var windowPosition = window.PointToScreen(new Point());

                elementWindow.WindowStartupLocation = WindowStartupLocation.Manual;
                elementWindow.Top = windowPosition.Y + 100;
                elementWindow.Left = windowPosition.X + 200;
                elementWindow.Height = Math.Max(window.ActualHeight - 200, 100);
            }

            elementWindow.Show();
        }

        private void Button_SwitchMode_Click(object sender, RoutedEventArgs e)
        {
            if (AssociatedTaskView?.CurrentProgramDisplay != null)
            {
                var currentViewMode = AssociatedTaskView.CurrentProgramDisplay.CurrentViewMode;
                var nextViewMode = currentViewMode == ProgramViewMode.IO ? ProgramViewMode.Variables : ProgramViewMode.IO;

                AssociatedTaskView.CurrentProgramDisplay.CurrentViewMode = nextViewMode;
            }                    
        }

        private void Button_New_Click(object sender, RoutedEventArgs e)
        {
            Log.Instance.Header("New button clicked");

            if (AssociatedTaskView?.CurrentProgramDisplay != null)
            {
                _ = StandardLocations.Instance.SetPathTo(Location.SaveFile, null);
                _ = ProgramHolder.Instance.RemoveProgram(AssociatedTaskView?.CurrentProgramDisplay?.AssociatedProgram);

                AssociatedTaskView.CurrentProgramDisplay.AssociatedProgram = ProgramHolder.Instance.NewProgram();
            }
        }

        private async void Button_Open_Click(object sender, RoutedEventArgs e)
        {
            Log.Instance.Header("Open button clicked");

            if (AssociatedTaskView?.CurrentProgramDisplay != null)
            {
                var program = await ProgramSaveLoad.Instance.LoadProgramDialogAsync(ProgramHolder.Instance, new DisplayProgramJsonStorer(new FunctionBuilder(), LoadedPlugins.Instance), Window.GetWindow(this));
                if (program != null)
                {
                    _ = ProgramHolder.Instance.RemoveProgram(AssociatedTaskView?.CurrentProgramDisplay?.AssociatedProgram);
                    AssociatedTaskView.CurrentProgramDisplay.AssociatedProgram = program;
                }
            }
        }

        private async void Button_Save_Click(object sender, RoutedEventArgs e)
        {
            Log.Instance.Header("Save button clicked");

            _ = await ProgramSaveLoad.Instance.TrySaveProgramFromMetadataAsync(AssociatedTaskView?.CurrentProgramDisplay?.AssociatedProgram, new DisplayProgramJsonStorer(new FunctionBuilder(), LoadedPlugins.Instance), Window.GetWindow(this));
        }

        private async void Button_SaveAs_Click(object sender, RoutedEventArgs e)
        {
            Log.Instance.Header("Save as button clicked");

            _ = await ProgramSaveLoad.Instance.SaveProgramDialogAsync(AssociatedTaskView?.CurrentProgramDisplay?.AssociatedProgram, new DisplayProgramJsonStorer(new FunctionBuilder(), LoadedPlugins.Instance), Window.GetWindow(this));
        }

        private void Button_TaskViewer_Click(object sender, RoutedEventArgs e)
        {
            AssociatedTaskView?.ShowTasks();
        }

        private void Button_Settings_Click(object sender, RoutedEventArgs e)
        {
            Log.Instance.Header("Settings button clicked");

            var settingsWindow = new SettingsWindow(CommonOptionsSaveLoad.Instance.OptionsStorer)
            {
                Owner = Window.GetWindow(this)
            };

            settingsWindow.Show();
        }
    }
}
