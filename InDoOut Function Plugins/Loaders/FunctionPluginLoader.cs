using InDoOut_Core.Plugins;
using InDoOut_Function_Plugins.Containers;
using InDoOut_Plugins.Containers;
using InDoOut_Plugins.Loaders;

namespace InDoOut_Function_Plugins.Loaders
{
    public class FunctionPluginLoader : PluginLoader
    {
        protected override IPluginContainer CreateContainer(IPlugin plugin)
        {
            return plugin != null ? new FunctionPluginContainer(plugin) : null;
        }
    }
}
