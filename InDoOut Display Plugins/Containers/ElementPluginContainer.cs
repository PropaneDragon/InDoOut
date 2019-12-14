using InDoOut_Core.Plugins;
using InDoOut_Display_Core.Elements;
using InDoOut_Display_Core.Functions;
using InDoOut_Plugins.Containers;
using System;
using System.Collections.Generic;

namespace InDoOut_Display_Plugins.Containers
{
    /// <summary>
    /// A plugin container for any plugin that can be assigned by <see cref="IElement"/>.
    /// </summary>
    public class ElementPluginContainer : PluginContainer, IElementPluginContainer
    {
        /// <summary>
        /// The UI elements the plugin makes available.
        /// </summary>
        public List<Type> DisplayElementTypes { get; } = new List<Type>();

        /// <summary>
        /// The element functions that this plugin makes visible.
        /// </summary>
        public List<Type> ElementFunctionTypes { get; } = new List<Type>();

        /// <summary>
        /// Creates a plugin container for storing plugins that can be assigned to a <see cref="IElement"/>.
        /// </summary>
        /// <param name="plugin"></param>
        public ElementPluginContainer(IPlugin plugin) : base(plugin)
        {
        }

        /// <summary>
        /// Inspects the incoming type and checks whether it can be assigned from an <see cref="IElement"/>.
        /// </summary>
        /// <param name="type"></param>
        protected override void InspectType(Type type)
        {
            if (type != null)
            {
                CheckAssignableAndAdd<IElementFunction>(ElementFunctionTypes, type);
                CheckAssignableAndAdd<IDisplayElement>(DisplayElementTypes, type);
            }
        }
    }
}
