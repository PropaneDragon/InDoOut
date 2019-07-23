using System.Reflection;

namespace InDoOut_Desktop.Plugins
{
    internal interface IPluginLoader
    {
        IPluginContainer LoadPlugin(Assembly assembly);
    }
}
