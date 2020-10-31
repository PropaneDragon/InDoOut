using InDoOut_Plugins.Loaders;
using System.Windows;

namespace InDoOut_UI_Common.Windows
{
    public partial class PluginsWindow : Window
    {
        public PluginsWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e) => ListView_Plugins.ItemsSource = LoadedPlugins.Instance.Plugins;

        private void Button_Close_Click(object sender, RoutedEventArgs e) => Close();
    }
}
