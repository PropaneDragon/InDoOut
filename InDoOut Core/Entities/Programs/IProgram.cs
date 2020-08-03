using InDoOut_Core.Basic;
using InDoOut_Core.Entities.Core;
using InDoOut_Core.Entities.Functions;
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
        /// Whether any functions in the program are still stopping.
        /// </summary>
        bool Stopping { get; }

        /// <summary>
        /// The return code of this program after execution. If there are any
        /// <see cref="IEndFunction"/>s present as part of this program and they have
        /// executed with a value in the <see cref="IEndFunction.ReturnCode"/> then that
        /// will be represented in the return code of this program.
        /// </summary>
        string ReturnCode { get; }

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
        /// All <see cref="IEndFunction"/>s that are the potential end results of this
        /// program.
        /// </summary>
        List<IEndFunction> EndFunctions { get; }

        /// <summary>
        /// Values to pass into all <see cref="StartFunctions"/> when the program is started.
        /// </summary>
        List<string> PassthroughValues { get; }

        /// <summary>
        /// Sends a request to all functions in the program to stop running. This is controlled by
        /// <see cref="IFunction.PolitelyStop"/>, so functions may take time to fully stop running.
        /// </summary>
        void Stop();

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
