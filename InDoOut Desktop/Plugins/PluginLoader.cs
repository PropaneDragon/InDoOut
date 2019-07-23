using InDoOut_Desktop_API.Plugins;
using System;
using System.Linq;
using System.Reflection;

namespace InDoOut_Desktop.Plugins
{
    internal class PluginLoader : IPluginLoader
    {
        public IPluginContainer LoadPlugin(Assembly assembly)
        {
            if (assembly != null)
            {
                var plugin = FindPlugin(assembly);
                if (plugin != null)
                {
                    return CreateContainer(plugin);
                }
            }

            return null;
        }

        protected virtual IPluginContainer CreateContainer(IPlugin plugin)
        {
            if (plugin != null)
            {
                return new PluginContainer(plugin);
            }

            return null;
        }

        private IPlugin FindPlugin(Assembly assembly)
        {
            if (assembly != null)
            {
                var validPluginType = assembly.GetExportedTypes().FirstOrDefault(type => typeof(IPlugin).IsAssignableFrom(type));
                if (validPluginType != null)
                {
                    try
                    {
                        var validConstructor = validPluginType.GetConstructor(Type.EmptyTypes);
                        if (validConstructor != null)
                        {
                            if (validConstructor.Invoke(new object[] { }) is IPlugin validPlugin)
                            {
                                return validPlugin;
                            }
                        }
                    }
                    catch { }
                }
            }

            return null;
        }
    }
}
