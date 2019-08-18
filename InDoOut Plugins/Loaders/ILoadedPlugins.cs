using InDoOut_Core.Instancing;
using InDoOut_Plugins.Containers;
using System;
using System.Collections.Generic;

namespace InDoOut_Plugins.Loaders
{
    /// <summary>
    /// Represents a storage area for loaded <see cref="IPluginContainer"/>s.
    /// </summary>
    public interface ILoadedPlugins : ISingleton<ILoadedPlugins>
    {
        /// <summary>
        /// Fired when a plugin has changed.
        /// </summary>
        event EventHandler<EventArgs> PluginsChanged;

        /// <summary>
        /// All currently loaded plugins.
        /// </summary>
        List<IPluginContainer> Plugins { get; set; }
    }
}
