using InDoOut_Core.Entities.Functions;
using InDoOut_Core.Functions;
using InDoOut_Display.UI.Controls.Screens;
using InDoOut_Function_Plugins.Containers;
using InDoOut_Plugins.Loaders;
using InDoOut_UI_Common.InterfaceElements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace InDoOut_Display.UI.Controls.FunctionSelector
{
    public partial class FunctionSelector : UserControl
    {
        private readonly IFunctionBuilder _functionBuilder = new FunctionBuilder();
        private IScreenConnections _screenConnections = null;

        public bool CloseWindowOnSelection { get; set; } = true;
        public IScreenConnections Screen { get => _screenConnections; set => UpdateScreen(value); }

        public FunctionSelector()
        {
            InitializeComponent();
        }

        private async Task RefreshPlugins()
        {
            var plugins = LoadedPlugins.Instance.Plugins;
            if (plugins != null && _functionBuilder != null)
            {
                var functionTypes = await Task.Run(() => plugins.Where(pluginContainer => pluginContainer is IFunctionPluginContainer).Cast<IFunctionPluginContainer>().SelectMany(plugin => plugin.FunctionTypes).Distinct());
                var functions = new List<IFunction>();

                foreach (var type in functionTypes)
                {
                    var function = await Task.Run(() => _functionBuilder?.BuildInstance(type));
                    if (function != null)
                    {
                        functions.Add(function);
                    }
                }

                SetFunctions(functions);
            }
        }

        private void SetFunctions(List<IFunction> functions)
        {
            List_Items.ItemsSource = functions;
        }

        private void UpdateScreen(IScreenConnections screen)
        {
            _screenConnections = screen;
        }

        private async void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            LoadedPlugins.Instance.PluginsChanged += Instance_PluginsChanged;

            await RefreshPlugins();
        }

        private void UserControl_Unloaded(object sender, System.Windows.RoutedEventArgs e)
        {
            LoadedPlugins.Instance.PluginsChanged -= Instance_PluginsChanged;
        }

        private async void Instance_PluginsChanged(object sender, EventArgs e)
        {
            await RefreshPlugins();
        }

        private void List_Items_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (List_Items.SelectedItem is IFunction selectedFunction)
            {
                var newFunctionInstance = _functionBuilder.BuildInstance(selectedFunction.GetType());
                if (newFunctionInstance != null && _screenConnections != null && _screenConnections is IFunctionDisplay functionDisplay)
                {
                    _ = functionDisplay.Create(newFunctionInstance);

                    if (CloseWindowOnSelection)
                    {
                        var window = Window.GetWindow(this);
                        if (window != null)
                        {
                            window.Close();
                        }
                    }
                }
            }
        }
    }
}
