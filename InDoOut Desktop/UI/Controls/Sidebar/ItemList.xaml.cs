using InDoOut_Core.Entities.Functions;
using InDoOut_Core.Functions;
using InDoOut_Core.Instancing;
using InDoOut_Core.Logging;
using InDoOut_Desktop.UI.Interfaces;
using InDoOut_Desktop.UI.Threading;
using InDoOut_Executable_Core.Messaging;
using InDoOut_Function_Plugins.Containers;
using InDoOut_Plugins.Loaders;
using InDoOut_UI_Common.InterfaceElements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

namespace InDoOut_Desktop.UI.Controls.Sidebar
{
    public partial class ItemList : UserControl, IFunctionList
    {
        private List<IFunction> _functions = null;

        public IFunctionDisplay FunctionView { get; set; } = null;
        public List<IFunction> Functions { get => _functions; set => SetFunctions(value); }

        public ItemList()
        {
            InitializeComponent();

            LoadedPlugins.Instance.PluginsChanged += Instance_PluginsChanged;
        }

        public void Filter(string search)
        {
            if (string.IsNullOrEmpty(search))
            {
                SetFunctions(_functions);
            }
            else
            {
                var lowercaseSearch = search.ToLower();
                var filteredFunctions = _functions.Where(function => (function?.SafeName?.ToLower().Contains(lowercaseSearch) ?? false) || (function?.SafeKeywords?.Any(keyword => keyword.ToLower().Contains(lowercaseSearch)) ?? false) || (function?.SafeGroup?.ToLower().Contains(lowercaseSearch) ?? false)).ToList();

                SetFunctions(filteredFunctions, true);
            }
        }

        public void ClearFilter()
        {
            Filter("");
        }

        private void SetFunctions(List<IFunction> functions, bool temporary = false)
        {
            if (!temporary)
            {
                _functions = functions;
            }

            List_Items.ItemsSource = functions;

            var collectionView = CollectionViewSource.GetDefaultView(List_Items.ItemsSource);
            collectionView.SortDescriptions.Clear();
            collectionView.SortDescriptions.Add(new System.ComponentModel.SortDescription("SafeGroup", System.ComponentModel.ListSortDirection.Ascending));
            collectionView.SortDescriptions.Add(new System.ComponentModel.SortDescription("SafeName", System.ComponentModel.ListSortDirection.Ascending));
            collectionView.GroupDescriptions.Clear();
            collectionView.GroupDescriptions.Add(new PropertyGroupDescription("SafeGroup"));
        }

        private async void Instance_PluginsChanged(object sender, EventArgs e)
        {
            Log.Instance.Header($"REBUILDING PLUGIN LIST ON SIDEBAR");

            if (sender is LoadedPlugins loadedPlugins)
            {
                var plugins = loadedPlugins.Plugins;
                var allTypes = await Task.Run(() => plugins.Where(pluginContainer => pluginContainer is IFunctionPluginContainer).Cast<IFunctionPluginContainer>().SelectMany(plugin => plugin.FunctionTypes).Distinct());
                var functionBuilder = new FunctionBuilder();
                var functions = new List<IFunction>();

                foreach (var type in allTypes)
                {
                    var function = await Task.Run(() => functionBuilder.BuildInstance(type));
                    if (function != null)
                    {
                        functions.Add(function);
                    }
                }

                _ = UIThread.Instance.TryRunOnUI(() => SetFunctions(functions));

                Log.Instance.Header($"DONE REBUILDING PLUGIN LIST ON SIDEBAR");
            }
        }

        private async void List_Items_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (List_Items.SelectedItem is IFunction selectedItem && FunctionView != null)
            {
                var functionType = selectedItem.GetType();
                var functionBuilder = new InstanceBuilder<IFunction>();
                var function = await Task.Run(() => functionBuilder.BuildInstance(functionType));

                if (function != null)
                {
                    var uiFunction = FunctionView.Create(function);
                    if (uiFunction == null)
                    {
                        Log.Instance.Error("UI Function for ", function, " couldn't be created on the interface");
                        UserMessageSystemHolder.Instance.CurrentUserMessageSystem?.ShowError("Unable to create function", "The function couldn't be placed in the program due to an unknown reason.");
                    }
                }
                else
                {
                    Log.Instance.Error("Couldn't build a function for ", functionType, " to place on the interface");
                    UserMessageSystemHolder.Instance.CurrentUserMessageSystem?.ShowError("Unable to create function", "The function couldn't be created due to an unknown reason.");
                }
            }
        }
    }
}
