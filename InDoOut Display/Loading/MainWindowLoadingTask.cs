using System;
using System.Threading.Tasks;
using InDoOut_Core.Logging;
using InDoOut_Display_Plugins.Loaders;
using InDoOut_Executable_Core.Loading;
using InDoOut_Executable_Core.Location;
using InDoOut_Function_Plugins.Loaders;
using InDoOut_Plugins.Loaders;
using InDoOut_UI_Common.SaveLoad;

namespace InDoOut_Display.Loading
{
    internal class MainWindowLoadingTask : LoadingTask
    {
        protected override async Task<bool> RunTaskAsync()
        {
            Log.Instance.Header($"PLUGIN LOADING BEGINNING");

            Name = "Loading plugins...";

            if (LoadedPlugins.Instance.Plugins.Count <= 0)
            {
                var allPluginsLoaded = true;

                allPluginsLoaded = await LoadPlugins(new ElementPluginLoader()) && allPluginsLoaded;
                allPluginsLoaded = await LoadPlugins(new FunctionPluginLoader()) && allPluginsLoaded;

                Name = "Plugins loaded.";

                Log.Instance.Header($"PLUGIN LOADING DONE");

                Name = "Loading options.";

                _ = await CommonOptionsSaveLoad.Instance.LoadAllOptionsAsync();

                Name = "Options loaded.";

                return allPluginsLoaded;
            }

            Name = "Plugin load failed.";

            Log.Instance.Header($"PLUGIN LOADING FAILED");

            await Task.Delay(TimeSpan.FromSeconds(5));

            return false;
        }

        private async Task<bool> LoadPlugins(IPluginLoader loader)
        {
            if (loader != null)
            {
                var pluginDirectoryLoader = new PluginDirectoryLoader(loader, StandardLocations.Instance);

                loader.PluginLoadSuccess += PluginLoader_OnPluginLoadSuccess;

                var pluginContainers = await pluginDirectoryLoader.LoadPlugins();

                foreach (var pluginContainer in pluginContainers)
                {
                    Name = $"Initialising {pluginContainer?.Plugin?.SafeName ?? "unknown"}...";
                    _ = await Task.Run(() => pluginContainer.Initialise());
                }

                loader.PluginLoadSuccess -= PluginLoader_OnPluginLoadSuccess;

                var currentPlugins = LoadedPlugins.Instance.Plugins;
                currentPlugins.AddRange(pluginContainers);

                LoadedPlugins.Instance.Plugins = currentPlugins;

                return true;
            }

            return false;
        }

        private void PluginLoader_OnPluginLoadSuccess(object sender, PluginLoadEventArgs e)
        {
            Name = $"Loaded {e.Path ?? "Unknown path"}";
        }
    }
}
