using InDoOut_Plugins.Containers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InDoOut_Desktop.Plugins
{
    internal interface IPluginDirectoryLoader
    {
        Task<List<IPluginContainer>> LoadPlugins();
        Task<List<IPluginContainer>> LoadPlugins(string directory);
    }
}
