using InDoOut_Plugins.Containers;
using InDoOut_Plugins.Core;
using System;
using System.Linq;
using System.Reflection;

namespace InDoOut_Plugins.Loaders
{
    /// <summary>
    /// Loads plugins as <see cref="IPluginContainer"/>s.
    /// </summary>
    public class PluginLoader : IPluginLoader
    {
        /// <summary>
        /// Triggered when a plugin has begun loading.
        /// </summary>
        public event EventHandler<PluginLoadEventArgs> OnPluginLoading;

        /// <summary>
        /// Triggered when a plugin has successfully loaded.
        /// </summary>
        public event EventHandler<PluginLoadEventArgs> OnPluginLoadSuccess;

        /// <summary>
        /// Triggered when a plugin has failed to load.
        /// </summary>
        public event EventHandler<PluginLoadEventArgs> OnPluginLoadFail;

        /// <summary>
        /// Loads a plugin from a given assembly path.
        /// </summary>
        /// <param name="path">The path to the assembly to be loaded.</param>
        /// <returns>A <see cref="IPluginContainer"/>, if valid. Returns null otherwise.</returns>
        public IPluginContainer LoadPlugin(string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                try
                {
                    var loadedAssembly = Assembly.LoadFrom(path);
                    if (loadedAssembly != null)
                    {
                        return LoadPlugin(loadedAssembly);
                    }
                }
                catch { }
            }

            return null;
        }

        /// <summary>
        /// Loads a plugin from a given assembly.
        /// </summary>
        /// <param name="assembly">The assembly to load.</param>
        /// <returns>A <see cref="IPluginContainer"/>, if valid. Returns null otherwise.</returns>
        public IPluginContainer LoadPlugin(Assembly assembly)
        {
            if (assembly != null)
            {
                OnPluginLoading?.Invoke(this, new PluginLoadEventArgs(this, assembly));

                var plugin = FindPlugin(assembly);
                if (plugin != null)
                {
                    OnPluginLoadSuccess?.Invoke(this, new PluginLoadEventArgs(this, assembly));

                    return CreateContainer(plugin);
                }
                else
                {
                    OnPluginLoadFail?.Invoke(this, new PluginLoadEventArgs(this, assembly));
                }
            }

            return null;
        }

        /// <summary>
        /// Creates a container for a given plugin.
        /// </summary>
        /// <param name="plugin">The plugin to containerise.</param>
        /// <returns>A plugin container for the plugin, if valid. Otherwise null.</returns>
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
