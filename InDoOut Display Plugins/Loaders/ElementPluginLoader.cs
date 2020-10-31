using InDoOut_Core.Plugins;
using InDoOut_Display_Plugins.Containers;
using InDoOut_Plugins.Containers;
using InDoOut_Plugins.Loaders;

namespace InDoOut_Display_Plugins.Loaders
{
    /// <summary>
    /// A plugin loader that can load in <see cref="ElementPluginContainer"/>s.
    /// </summary>
    public class ElementPluginLoader : PluginLoader
    {
        /// <summary>
        /// Creates an element container for a given plugin.
        /// </summary>
        /// <param name="plugin">The plugin to create a container for.</param>
        /// <returns>The container generated from the given plugin.</returns>
        protected override IPluginContainer CreateContainer(IPlugin plugin) => plugin != null ? new ElementPluginContainer(plugin) : null;
    }
}
