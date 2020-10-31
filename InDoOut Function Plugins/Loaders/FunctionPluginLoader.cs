using InDoOut_Core.Plugins;
using InDoOut_Function_Plugins.Containers;
using InDoOut_Plugins.Containers;
using InDoOut_Plugins.Loaders;

namespace InDoOut_Function_Plugins.Loaders
{
    /// <summary>
    /// A plugin loader that can load in <see cref="FunctionPluginContainer"/>s.
    /// </summary>
    public class FunctionPluginLoader : PluginLoader
    {
        /// <summary>
        /// Creates a function container for a given plugin.
        /// </summary>
        /// <param name="plugin">The plugin to create a container for.</param>
        /// <returns>The generated container from the given plugin.</returns>
        protected override IPluginContainer CreateContainer(IPlugin plugin) => plugin != null ? new FunctionPluginContainer(plugin) : null;
    }
}
