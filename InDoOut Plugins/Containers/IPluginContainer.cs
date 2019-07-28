using InDoOut_Core.Entities.Functions;
using InDoOut_Plugins.Core;
using System;
using System.Collections.Generic;

namespace InDoOut_Plugins.Containers
{
    /// <summary>
    /// Represents a container that contains metadata for a standard <see cref="IPlugin"/>.
    /// </summary>
    public interface IPluginContainer
    {
        /// <summary>
        /// The container contains a valid plugin.
        /// </summary>
        bool Valid { get; }

        /// <summary>
        /// The plugin that this container houses.
        /// </summary>
        IPlugin Plugin { get; }

        /// <summary>
        /// The functions that this plugin makes visible.
        /// </summary>
        List<Type> FunctionTypes { get; }

        /// <summary>
        /// Initialises the plugin container, which populates the metadata.
        /// </summary>
        /// <returns>Whether or not the container could be initialised.</returns>
        bool Initialise();
    }
}
