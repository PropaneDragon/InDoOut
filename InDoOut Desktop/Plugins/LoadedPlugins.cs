using System;
using System.Collections.Generic;
using InDoOut_Core.Instancing;
using InDoOut_Plugins.Containers;

namespace InDoOut_Desktop.Plugins
{
    internal class LoadedPlugins : Singleton<LoadedPlugins>, ILoadedPlugins
    {
        private List<IPluginContainer> _plugins = new List<IPluginContainer>();

        public event EventHandler<EventArgs> PluginsChanged;

        public List<IPluginContainer> Plugins { get => _plugins; set => SetPlugins(value); }

        private void SetPlugins(List<IPluginContainer> plugins)
        {
            _plugins = plugins;

            PluginsChanged?.Invoke(this, new EventArgs());
        }
    }
}
