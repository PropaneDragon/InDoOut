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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace InDoOut_Desktop.UI.Controls.Sidebar
{
    public partial class ItemList : UserControl, IFunctionList
    {
        private List<IFunction> _functions = null;
        private Point _startPoint = new();

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

        public void ClearFilter() => Filter("");

        private void SetFunctions(List<IFunction> functions, bool temporary = false)
        {
            if (!temporary)
            {
                _functions = functions;
            }

            var listCollectionView = new ListCollectionView(functions)
            {
                CustomSort = Comparer<IFunction>.Create((a, b) => a.SafeGroup.CompareTo(b.SafeGroup) + a.SafeName.CompareTo(b.SafeName))
            };

            listCollectionView.GroupDescriptions.Clear();
            listCollectionView.GroupDescriptions.Add(new PropertyGroupDescription("SafeGroup"));

            List_Items.ItemsSource = listCollectionView;
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

        private async void List_Items_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (List_Items.SelectedItem is IFunction selectedItem && FunctionView != null)
            {
                var functionType = selectedItem.GetType();
                var functionBuilder = new InstanceBuilder<IFunction>();
                var function = await Task.Run(() => functionBuilder.BuildInstance(functionType));

                if (function != null)
                {
                    var uiFunction = FunctionView?.FunctionCreator?.Create(function);
                    if (uiFunction == null)
                    {
                        Log.Instance.Error("UI Function for ", function, " couldn't be created on the interface");
                        UserMessageSystemHolder.Instance.CurrentUserMessageSystem?.ShowError("Unable to create function", "The selected function doesn't appear to be able to be placed in the current program.");
                    }
                }
                else
                {
                    Log.Instance.Error("Couldn't build a function for ", functionType, " to place on the interface");
                    UserMessageSystemHolder.Instance.CurrentUserMessageSystem?.ShowError("Unable to create function", "The selected function couldn't be created and can't be placed in the current program.");
                }
            }
        }

        private void List_Items_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = false;

            _startPoint = e.GetPosition(null);
        } 

        private void List_Items_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            e.Handled = false;

            if (sender is ListView listView && e.LeftButton == MouseButtonState.Pressed)
            {
                var position = e.GetPosition(null);
                var distance = _startPoint - position;

                if (Math.Abs(distance.X) > SystemParameters.MinimumHorizontalDragDistance || Math.Abs(distance.Y) > SystemParameters.MinimumVerticalDragDistance)
                {
                    if (List_Items.SelectedItem is IFunction selectedItem)
                    {
                        var data = new DataObject("Function", selectedItem);
                        _ = DragDrop.DoDragDrop(listView, data, DragDropEffects.Copy);
                    }
                }
            }
        }
    }
}
