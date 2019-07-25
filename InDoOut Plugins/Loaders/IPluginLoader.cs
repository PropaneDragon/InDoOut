using InDoOut_Plugins.Containers;
using System.Reflection;

namespace InDoOut_Plugins.Loaders
{
    /// <summary>
    /// Represents a loader that can load <see cref="IPluginContainer"/>s.
    /// </summary>
    public interface IPluginLoader
    {
        /// <summary>
        /// Loads a plugin from a given assembly path.
        /// </summary>
        /// <param name="path">The path to the assembly to be loaded.</param>
        /// <returns>A <see cref="IPluginContainer"/>, if valid. Returns null otherwise.</returns>
        IPluginContainer LoadPlugin(string path);

        /// <summary>
        /// Loads a plugin from a given assembly.
        /// </summary>
        /// <param name="assembly">The assembly to load.</param>
        /// <returns>A <see cref="IPluginContainer"/>, if valid. Returns null otherwise.</returns>
        IPluginContainer LoadPlugin(Assembly assembly);
    }
}
