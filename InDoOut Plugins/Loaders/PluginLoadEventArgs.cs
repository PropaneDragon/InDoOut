using System;
using System.Reflection;

namespace InDoOut_Plugins.Loaders
{
    /// <summary>
    /// Event arguments for a plugin load event.
    /// </summary>
    public class PluginLoadEventArgs : EventArgs
    {
        /// <summary>
        /// The path to the currently loading plugin.
        /// </summary>
        public string Path { get; private set; } = null;

        /// <summary>
        /// The plugin loader that is currently loading the plugin.
        /// </summary>
        public IPluginLoader PluginLoader { get; private set; } = null;

        /// <summary>
        /// Creates event args given a <see cref="IPluginLoader"/> and a path to a plugin.
        /// </summary>
        /// <param name="loader">The loader that is currently loading the plugin.</param>
        /// <param name="path">The path to the plugin that is currently loading.</param>
        public PluginLoadEventArgs(IPluginLoader loader, string path)
        {
            PluginLoader = loader;
            Path = path;
        }

        /// <summary>
        /// Creates event args given a <see cref="IPluginLoader"/> and an <see cref="Assembly"/>.
        /// </summary>
        /// <param name="loader">The loader that is currently loading the plugin.</param>
        /// <param name="assembly">The assembly that is currently being loaded.</param>
        public PluginLoadEventArgs(IPluginLoader loader, Assembly assembly) : this(loader, assembly?.Location)
        {
        }
    }
}
