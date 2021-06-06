using InDoOut_Core.Logging;
using InDoOut_Executable_Core.Loading;
using InDoOut_Executable_Core.Location;
using InDoOut_Function_Plugins.Loaders;
using InDoOut_Plugins.Loaders;
using System.Threading.Tasks;

namespace InDoOut_Viewer.Loading
{
    public class MainWindowLoadingTask : LoadingTask
    {
        protected override async Task<bool> RunTaskAsync()
        {
            Log.Instance.Header($"PLUGIN LOADING BEGINNING");

            Name = "Loading plugins...";

            if (LoadedPlugins.Instance.Plugins.Count <= 0)
            {
                var pluginLoader = new FunctionPluginLoader();
                var pluginDirectoryLoader = new PluginDirectoryLoader(pluginLoader, StandardLocations.Instance);

                pluginLoader.PluginLoadSuccess += PluginLoader_OnPluginLoadSuccess;

                var pluginContainers = await pluginDirectoryLoader.LoadPlugins();

                foreach (var pluginContainer in pluginContainers)
                {
                    Name = $"Initialising {pluginContainer?.Plugin?.SafeName ?? "unknown"}...";
                    _ = await Task.Run(() => pluginContainer.Initialise());
                }

                LoadedPlugins.Instance.Plugins = pluginContainers;

                Name = "Plugins loaded.";

                Log.Instance.Header($"PLUGIN LOADING DONE");
            }

            return true;
        }

        private void PluginLoader_OnPluginLoadSuccess(object sender, PluginLoadEventArgs e) => Name = $"Loaded {e.Path ?? "Unknown path"}";
    }
}
