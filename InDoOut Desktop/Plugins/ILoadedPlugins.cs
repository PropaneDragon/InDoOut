using InDoOut_Core.Instancing;
using InDoOut_Plugins.Containers;
using System.Collections.Generic;

namespace InDoOut_Desktop.Plugins
{
    internal interface ILoadedPlugins : ISingleton<ILoadedPlugins>
    {
        List<IPluginContainer> Plugins { get; }
    }
}
