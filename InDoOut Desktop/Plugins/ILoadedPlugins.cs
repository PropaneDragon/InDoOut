using InDoOut_Core.Instancing;
using InDoOut_Plugins.Containers;
using System;
using System.Collections.Generic;

namespace InDoOut_Desktop.Plugins
{
    internal interface ILoadedPlugins : ISingleton<ILoadedPlugins>
    {
        event EventHandler<EventArgs> PluginsChanged;

        List<IPluginContainer> Plugins { get; set; }
    }
}
