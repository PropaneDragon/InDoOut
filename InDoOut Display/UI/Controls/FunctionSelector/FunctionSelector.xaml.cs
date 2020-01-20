using InDoOut_Core.Entities.Functions;
using InDoOut_Core.Functions;
using InDoOut_Function_Plugins.Containers;
using InDoOut_Plugins.Loaders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace InDoOut_Display.UI.Controls.FunctionSelector
{
    public partial class FunctionSelector : UserControl
    {
        private IFunctionBuilder _functionBuilder = new FunctionBuilder();

        public FunctionSelector()
        {
            InitializeComponent();
        }

        private async Task RefreshPlugins()
        {
            var plugins = LoadedPlugins.Instance.Plugins;
            if (plugins != null)
            {
                var functionTypes = await Task.Run(() => plugins.Where(pluginContainer => pluginContainer is IFunctionPluginContainer).Cast<IFunctionPluginContainer>().SelectMany(plugin => plugin.FunctionTypes).Distinct());
                var functionBuilder = new FunctionBuilder();
                var functions = new List<IFunction>();

                foreach (var type in functionTypes)
                {
                    var function = await Task.Run(() => functionBuilder.BuildInstance(type));
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

        }
    }
}
