using System;
using System.Collections.Generic;

namespace InDoOut_Function_Plugins.Containers
{
    /// <summary>
    /// Represents a plugin container for Function types.
    /// </summary>
    public interface IFunctionPluginContainer
    {
        /// <summary>
        /// The functions that this plugin makes visible.
        /// </summary>
        List<Type> FunctionTypes { get; }
    }
}