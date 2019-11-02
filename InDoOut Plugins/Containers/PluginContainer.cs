using InDoOut_Core.Entities.Functions;
using InDoOut_Core.Plugins;
using System;
using System.Collections.Generic;

namespace InDoOut_Plugins.Containers
{
    /// <summary>
    /// A container that contains metadata for a standard <see cref="IPlugin"/>.
    /// </summary>
    public class PluginContainer : IPluginContainer
    {
        /// <summary>
        /// Whether or not the container has a valid plugin.
        /// </summary>
        public bool Valid => Plugin != null && Plugin.Valid;

        /// <summary>
        /// The plugin this container houses.
        /// </summary>
        public IPlugin Plugin { get; private set; } = null;

        /// <summary>
        /// The functions the plugin makes available.
        /// </summary>
        public List<Type> FunctionTypes { get; } = new List<Type>();

        /// <summary>
        /// Creates a standard plugin container with an empty plugin.
        /// </summary>
        protected PluginContainer()
        {
        }

        /// <summary>
        /// Creates a plugin container to house the given <paramref name="plugin"/>.
        /// </summary>
        /// <param name="plugin">The plugin this container belongs to.</param>
        public PluginContainer(IPlugin plugin) : this()
        {
            Plugin = plugin;
        }

        /// <summary>
        /// Initialises the metadata for the stored plugin.
        /// </summary>
        /// <returns>Whether or not the initialisation was a success.</returns>
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
                CheckAssignableAndAdd<IFunction>(FunctionTypes, type);
            }
        }

        private void CheckAssignableAndAdd<T>(List<Type> addTo, Type type) where T : class
        {
            if (typeof(T).IsAssignableFrom(type))
            {
                addTo.Add(type);
            }
        }
    }
}
