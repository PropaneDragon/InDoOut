using InDoOut_Core.Entities.Functions;
using InDoOut_Core.Plugins;
using InDoOut_Plugins.Containers;
using System;
using System.Collections.Generic;

namespace InDoOut_Function_Plugins.Containers
{
    /// <summary>
    /// A plugin container for any plugin that can be assigned by <see cref="IFunction"/>.
    /// </summary>
    public class FunctionPluginContainer : PluginContainer, IFunctionPluginContainer
    {
        /// <summary>
        /// The functions the plugin makes available.
        /// </summary>
        public List<Type> FunctionTypes { get; } = new List<Type>();

        /// <summary>
        /// Creates a plugin container for storing plugins that can be assigned to a <see cref="IFunction"/>.
        /// </summary>
        /// <param name="plugin"></param>
        public FunctionPluginContainer(IPlugin plugin) : base(plugin)
        {
        }

        /// <summary>
        /// Inspects the incoming type and checks whether it can be assigned from an <see cref="IFunction"/>.
        /// </summary>
        /// <param name="type"></param>
        protected override void InspectType(Type type)
        {
            if (type != null)
            {
                CheckAssignableAndAdd<IFunction>(FunctionTypes, type);
            }
        }
    }
}
