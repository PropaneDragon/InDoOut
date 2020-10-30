using InDoOut_Console_Common.ConsoleExtensions;
using InDoOut_Executable_Core.Location;
using InDoOut_Function_Plugins.Loaders;
using InDoOut_Plugins.Loaders;

namespace InDoOut_Console_Common.Loading
{
    public class ConsolePluginLoader
    {
        public bool LoadPlugins()
        {
            ExtendedConsole.WriteLine();
            ConsoleFormatter.DrawSubtitle("Loading plugins");

            var pluginLoader = new PluginDirectoryLoader(new FunctionPluginLoader(), StandardLocations.Instance);
            var loadedPlugins = pluginLoader.LoadPlugins().Result;
            var pluginsLoaded = true;

            foreach (var plugin in loadedPlugins)
            {
                ConsoleFormatter.DrawInfoMessage("Loading plugin ", ConsoleFormatter.PurplePastel, $"{plugin?.Plugin?.SafeName ?? "Invalid plugin"}", ConsoleFormatter.Primary, "... ");

                if (plugin.Initialise())
                {
                    ExtendedConsole.WriteLine(ConsoleFormatter.GreenPastel, "Plugin loaded.");
                }
                else
                {
                    ExtendedConsole.WriteLine(ConsoleFormatter.RedPastel, "Plugin failed to load.");

                    pluginsLoaded = false;
                }
            }

            ExtendedConsole.WriteLine();

            LoadedPlugins.Instance.Plugins = loadedPlugins;

            return pluginsLoaded;
        }
    }
}
