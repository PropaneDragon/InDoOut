using InDoOut_Core.Plugins;
using System;
using System.Collections.Generic;

namespace InDoOut_Plugins.Containers
{
    /// <summary>
    /// A container that contains metadata for a standard <see cref="IPlugin"/>.
    /// </summary>
    public abstract class PluginContainer : IPluginContainer
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
        /// Creates a standard plugin container with an empty plugin.
        /// </summary>
        private PluginContainer()
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

        /// <summary>
        /// Checks a type given from an exported assembly type when importing a plugin. This allows for 
        /// types to be inspected and added to lists when <see cref="Initialise"/> is called.
        /// </summary>
        /// <param name="type"></param>
        protected abstract void InspectType(Type type);

        /// <summary>
        /// Checks whether the type <typeparamref name="T"/> is assignable from the given type <paramref name="type"/>.
        /// If it is then it will be added to the list given in <paramref name="addTo"/>.
        /// </summary>
        /// <typeparam name="T">The type to check if it can be assigned from <paramref name="type"/>.</typeparam>
        /// <param name="addTo">The list to insert the <paramref name="type"/> into if <typeparamref name="T"/> is assignable from it.</param>
        /// <param name="type">The type to check assignability against.</param>
        protected void CheckAssignableAndAdd<T>(List<Type> addTo, Type type) where T : class
        {
            if (typeof(T).IsAssignableFrom(type))
            {
                addTo.Add(type);
            }
        }
    }
}
