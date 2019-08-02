using InDoOut_Desktop.Location;
using InDoOut_Desktop.Plugins;
using InDoOut_Plugins.Loaders;
using System;
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

                pluginLoader.PluginLoadSuccess += PluginLoader_OnPluginLoadSuccess;

                var pluginContainers = await pluginDirectoryLoader.LoadPlugins();

                foreach (var pluginContainer in pluginContainers)
                {
                    Name = $"Initialising {pluginContainer?.Plugin?.SafeName ?? "unknown"}...";
                    await Task.Run(() => pluginContainer.Initialise());
                }

                LoadedPlugins.Instance.Plugins = pluginContainers;

                Name = "Plugins loaded.";

                await Task.Delay(TimeSpan.FromSeconds(3));

                return true;
            }

            Name = "Plugin load failed.";

            await Task.Delay(TimeSpan.FromSeconds(5));

            return false;
        }

        private void PluginLoader_OnPluginLoadSuccess(object sender, PluginLoadEventArgs e)
        {
            Name = $"Loaded {e.Path ?? "Unknown path"}";
        }
    }
}
