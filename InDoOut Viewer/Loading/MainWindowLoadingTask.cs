using InDoOut_Core.Logging;
using InDoOut_Executable_Core.Loading;
using InDoOut_Executable_Core.Location;
using InDoOut_Executable_Core.Messaging;
using InDoOut_Executable_Core.Options;
using InDoOut_Function_Plugins.Loaders;
using InDoOut_Plugins.Loaders;
using InDoOut_UI_Common.Messaging;
using InDoOut_UI_Common.SaveLoad;
using InDoOut_Viewer.Options;
using System;
using System.Threading.Tasks;

namespace InDoOut_Viewer.Loading
{
    public class MainWindowLoadingTask : LoadingTask
    {
        protected override async Task<bool> RunTaskAsync()
        {
            ProgramOptionsHolder.Instance.ProgramOptions = new ProgramOptions();
            UserMessageSystemHolder.Instance.CurrentUserMessageSystem = new DesktopUserMessageSystem();

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

                Log.Instance.Header($"PLUGIN LOADING DONE");

                Name = "Loading options...";

                _ = await CommonOptionsSaveLoad.Instance.LoadAllOptionsAsync();

                Name = "Welcome.";

                await Task.Delay(TimeSpan.FromSeconds(1));
            }

            return true;
        }

        private void PluginLoader_OnPluginLoadSuccess(object sender, PluginLoadEventArgs e) => Name = $"Loaded {e.Path ?? "Unknown path"}";
    }
}
