using System.Collections.Generic;
using InDoOut_Core.Instancing;
using InDoOut_Plugins.Containers;

namespace InDoOut_Desktop.Plugins
{
    internal class LoadedPlugins : Singleton<LoadedPlugins>, ILoadedPlugins
    {
        public List<IPluginContainer> Plugins { get; set; } = new List<IPluginContainer>();
    }
}
