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
        /// Values to pass into all <see cref="StartFunctions"/> when the program is started.
        /// </summary>
        List<string> PassthroughValues { get; }

        /// <summary>
        /// The variable store for this program. This is where <see cref="IVariable"/>s are
        /// held for functions.
        /// </summary>
        IVariableStore VariableStore { get; }

        /// <summary>
        /// Sets the program name.
        /// </summary>
        /// <param name="name">The name to set.</param>
        void SetName(string name);

        /// <summary>
        /// Add a function to the program.
        /// </summary>
        /// <param name="function">The function to add to the program.</param>
        /// <returns>Whether the function was added.</returns>
        bool AddFunction(IFunction function);

        /// <summary>
        /// Removes a function from the program.
        /// </summary>
        /// <param name="function">The function to remove from the program.</param>
        /// <returns>Whether the function was found and removed.</returns>
        bool RemoveFunction(IFunction function);
    }
}
