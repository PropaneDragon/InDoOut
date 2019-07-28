using InDoOut_Core.Entities.Functions;
using InDoOut_Core.Instancing;
using InDoOut_Plugins.Loaders;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Data;

namespace InDoOut_Desktop.UI.Controls.Sidebar
{
    public partial class ItemList : UserControl
    {
        private List<IFunction> _functions = null;

        public List<IFunction> Plugins
        {
            get => _functions;
            set => SetFunctions(value);
        }

        public ItemList()
        {
            InitializeComponent();

            var loader = new PluginLoader();
            var container = loader.LoadPlugin(@"D:\Projects\InDoOut\App\InDoOut Desktop\bin\Debug\netcoreapp3.0\InDoOut Desktop API Tests.dll");

            container.Initialise();

            var functions = new List<IFunction>();
            var builder = new InstanceBuilder<IFunction>();

            foreach (var functionType in container.FunctionTypes)
            {
                var function = builder.BuildInstance(functionType);
                if (function != null)
                {
                    functions.Add(function);
                }
            }

            SetFunctions(functions);
        }

        private void SetFunctions(List<IFunction> functions)
        {
            _functions = functions;

            List_Items.ItemsSource = _functions;

            var collectionView = CollectionViewSource.GetDefaultView(List_Items.ItemsSource);
            collectionView.GroupDescriptions.Add(new PropertyGroupDescription("SafeGroup"));
        }
    }
}
