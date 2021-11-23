using InDoOut_Core.Instancing;
using InDoOut_Plugins.Containers;
using System;
using System.Collections.Generic;

namespace InDoOut_Plugins.Loaders
{
    /// <summary>
    /// A list of currently loaded plugins.
    /// </summary>
    public class LoadedPlugins : Singleton<LoadedPlugins>, ILoadedPlugins
    {
        private List<IPluginContainer> _plugins = new();

        /// <summary>
        /// An event that fires when plugins have changed, either by loading or unloading.
        /// </summary>
        public event EventHandler<EventArgs> PluginsChanged;

        /// <summary>
        /// The currently loaded plugins.
        /// </summary>
        public List<IPluginContainer> Plugins { get => _plugins; set => SetPlugins(value); }

        private void SetPlugins(List<IPluginContainer> plugins)
        {
            _plugins = plugins;

            PluginsChanged?.Invoke(this, new EventArgs());
        }
    }
}
