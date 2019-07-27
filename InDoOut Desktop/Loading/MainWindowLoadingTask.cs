using InDoOut_Desktop.Location;
using InDoOut_Desktop.Plugins;
using InDoOut_Plugins.Loaders;
using System.Threading.Tasks;

namespace InDoOut_Desktop.Loading
{
    internal class MainWindowLoadingTask : LoadingTask
    {
        public MainWindowLoadingTask()
        {
        }

        protected override async Task<bool> RunTaskAsync()
        {
            Name = "Loading plugins...";

            if (LoadedPlugins.Instance.Plugins.Count <= 0) {
                var pluginLoader = new PluginLoader();
                var pluginDirectoryLoader = new PluginDirectoryLoader(pluginLoader, StandardLocations.Instance);

                pluginLoader.OnPluginLoadSuccess += PluginLoader_OnPluginLoadSuccess;

                var plugins = await pluginDirectoryLoader.LoadPlugins();

                LoadedPlugins.Instance.Plugins = plugins;

                Name = "Plugins loaded.";
                return true;
            }

            Name = "Plugin load failed.";
            return false;
        }

        private void PluginLoader_OnPluginLoadSuccess(object sender, PluginLoadEventArgs e)
        {
            Name = $"Loaded {e.Path ?? "Unknown path"}";
        }
    }
}
