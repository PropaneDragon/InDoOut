using InDoOut_Core.Basic;
using InDoOut_Core.Entities.Core;
using InDoOut_Core.Entities.Functions;
using InDoOut_Core.Variables;
using System.Collections.Generic;

namespace InDoOut_Core.Entities.Programs
{
    /// <summary>
    /// Represents a group of self contained <see cref="IFunction"/>s that have
    /// interactivity between each other.
    /// </summary>
    public interface IProgram : ITriggerable<IEntity>, INamed, IStored
    {
        /// <summary>
        /// All <see cref="IFunction"/>s that are contained within this program.
        /// </summary>
        List<IFunction> Functions { get; }

        /// <summary>
        /// All <see cref="IStartFunction"/>s that are available to be started when the 
        /// program is started.
        /// </summary>
        List<IStartFunction> StartFunctions { get; }

        /// <summary>
        /// The variable store for this program. This is where <see cref="IVariable"/>s are
        /// held for functions.
        /// </summary>
        IVariableStore VariableStore { get; }

        /// <summary>
        /// Add a function to the program.
        /// </summary>
        /// <param name="function">The function to add to the program.</param>
        /// <returns>Whether the function was added.</returns>
        bool AddFunction(IFunction function);
    }
}
