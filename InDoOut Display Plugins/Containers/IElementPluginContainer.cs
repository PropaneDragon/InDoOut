using InDoOut_Plugins.Containers;
using System;
using System.Collections.Generic;
using System.Text;

namespace InDoOut_Display_Plugins.Containers
{
    /// <summary>
    /// Represents a plugin container for Element types.
    /// </summary>
    public interface IElementPluginContainer : IPluginContainer
    {
        /// <summary>
        /// The UI elements that this plugin makes visible.
        /// </summary>
        List<Type> DisplayElementTypes { get; }

        /// <summary>
        /// The element functions that this plugin makes visible.
        /// </summary>
        List<Type> ElementFunctionTypes { get; }
    }
}
