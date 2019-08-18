using InDoOut_Plugins.Containers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InDoOut_Plugins.Loaders
{
    /// <summary>
    /// Represents a loader that is capable of loading plugins from a directory location.
    /// </summary>
    public interface IPluginDirectoryLoader
    {
        /// <summary>
        /// Loads all plugins from a standard directory location.
        /// </summary>
        /// <returns>A list of plugins that have been loaded.</returns>
        Task<List<IPluginContainer>> LoadPlugins();

        /// <summary>
        /// Loads all plugins from a given directory location.
        /// </summary>
        /// <param name="directory">The directory to load plugins from.</param>
        /// <returns>A list of loaded plugins.</returns>
        Task<List<IPluginContainer>> LoadPlugins(string directory);
    }
}
