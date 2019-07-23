using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using InDoOut_Core.Entities.Functions;
using InDoOut_Desktop_API.Plugins;

[assembly: InternalsVisibleTo("InDoOut Desktop Tests")]
namespace InDoOut_Desktop.Plugins
{
    internal class PluginContainer : IPluginContainer
    {
        public bool Valid => Plugin != null && Plugin.Valid;

        public IPlugin Plugin { get; private set; } = null;

        public List<IFunction> Functions { get; } = new List<IFunction>();

        protected PluginContainer()
        {
        }

        public PluginContainer(IPlugin plugin) : this()
        {
            Plugin = plugin;
        }

        public bool Initialise()
        {
            if (Valid)
            {
                var assembly = Plugin.GetType().Assembly;
                if (assembly != null)
                {
                    var exportedTypes = assembly.GetExportedTypes();
                    foreach (var exportedType in exportedTypes)
                    {
                        InspectType(exportedType);
                    }

                    return true;
                }
            }

            return false;
        }

        private void InspectType(Type type)
        {
            if (type != null)
            {
                CheckAssignableAndAdd(Functions, type);
            }
        }

        private void CheckAssignableAndAdd<T>(List<T> addTo, Type type) where T : class
        {
            if (typeof(T).IsAssignableFrom(type))
            {
                addTo.Add(type as T);
            }
        }
    }
}
