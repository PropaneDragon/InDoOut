using InDoOut_Console_Common.ConsoleExtensions;
using InDoOut_Executable_Core.Location;
using InDoOut_Function_Plugins.Loaders;
using InDoOut_Plugins.Loaders;

namespace InDoOut_Console_Common.Loading
{
    public class ConsolePluginLoader : ConsoleLoader
    {
        public override string Name => "loading plugins";

        protected override bool BeginLoad()
        {
            var pluginLoader = new PluginDirectoryLoader(new FunctionPluginLoader(), StandardLocations.Instance);
            var loadedPlugins = pluginLoader.LoadPlugins().Result;
            var pluginsLoaded = true;

            foreach (var plugin in loadedPlugins)
            {
                pluginsLoaded = WriteMessageLine(() => plugin.Initialise(), "Loading plugin ", ConsoleFormatter.AccentTertiary, $"{plugin?.Plugin?.SafeName ?? "Invalid plugin"}", ConsoleFormatter.Primary, "... ") && pluginsLoaded;
            }

            LoadedPlugins.Instance.Plugins = loadedPlugins;

            return pluginsLoaded;
        }
    }
}
