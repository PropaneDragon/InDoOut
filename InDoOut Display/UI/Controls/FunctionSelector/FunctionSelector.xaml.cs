using InDoOut_Core.Entities.Functions;
using InDoOut_Core.Functions;
using InDoOut_Display_Core.Functions;
using InDoOut_Function_Plugins.Containers;
using InDoOut_Plugins.Loaders;
using InDoOut_UI_Common.InterfaceElements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Threading;

namespace InDoOut_Display.UI.Controls.FunctionSelector
{
    public partial class FunctionSelector : UserControl
    {
        private readonly IFunctionBuilder _functionBuilder = new FunctionBuilder();
        private ICommonProgramDisplay _programDisplay = null;
        private IEnumerable<Type> _previousFunctionTypes = null;
        private List<IFunction> _functions = null;
        private DispatcherTimer _reloadTimer = null;

        public bool CloseWindowOnSelection { get; set; } = true;
        public ICommonProgramDisplay ProgramDisplay { get => _programDisplay; set => UpdateProgramDisplay(value); }

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

                if (_previousFunctionTypes == null || _previousFunctionTypes.Count() != functionTypes.Count())
                {
                    foreach (var type in functionTypes)
                    {
                        if (!typeof(IElementFunction).IsAssignableFrom(type))
                        {
                            var function = await Task.Run(() => _functionBuilder?.BuildInstance(type));
                            if (function != null)
                            {
                                functions.Add(function);
                            }
                        }
                    }
                }

                _previousFunctionTypes = functionTypes;
                _functions = functions;

                SetFunctions(functions);
            }
        }

        private void SetFunctions(List<IFunction> functions)
        {
            List_Items.ItemsSource = functions;

            var collectionView = CollectionViewSource.GetDefaultView(List_Items.ItemsSource);
            collectionView.SortDescriptions.Clear();
            collectionView.SortDescriptions.Add(new System.ComponentModel.SortDescription("SafeGroup", System.ComponentModel.ListSortDirection.Ascending));
            collectionView.SortDescriptions.Add(new System.ComponentModel.SortDescription("SafeName", System.ComponentModel.ListSortDirection.Ascending));
            collectionView.GroupDescriptions.Clear();
            collectionView.GroupDescriptions.Add(new PropertyGroupDescription("SafeGroup"));
        }

        private void UpdateProgramDisplay(ICommonProgramDisplay programDisplay)
        {
            _programDisplay = programDisplay;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (_reloadTimer == null)
            {
                _reloadTimer = new DispatcherTimer(DispatcherPriority.Normal)
                {
                    Interval = TimeSpan.FromMilliseconds(0),
                    IsEnabled = true
                };

                _reloadTimer.Tick += ReloadTimer_Tick;
            }

            LoadedPlugins.Instance.PluginsChanged += Instance_PluginsChanged;
        }

        private async void ReloadTimer_Tick(object sender, EventArgs e)
        {
            _reloadTimer.Stop();
            _reloadTimer.Tick -= ReloadTimer_Tick;
            _reloadTimer = null;

            await RefreshPlugins();
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
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
                if (newFunctionInstance != null && _programDisplay != null)
                {
                    _ = _programDisplay.Create(newFunctionInstance);

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

        private void SearchBar_SearchRequested(object sender, InDoOut_UI_Common.Controls.Search.SearchArgs e)
        {
            if (_functions != null)
            {
                if (string.IsNullOrEmpty(e.Query))
                {
                    SetFunctions(_functions);
                }
                else
                {
                    var lowercaseSearch = e.Query.ToLower();
                    var filteredFunctions = _functions.Where(function => (function?.SafeName?.ToLower().Contains(lowercaseSearch) ?? false) || (function?.SafeKeywords?.Any(keyword => keyword.ToLower().Contains(lowercaseSearch)) ?? false) || (function?.SafeGroup?.ToLower().Contains(lowercaseSearch) ?? false)).ToList();

                    SetFunctions(filteredFunctions);
                }
            }
        }
    }
}
