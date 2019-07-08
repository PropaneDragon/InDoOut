using InDoOut_Core.Entities.Core;
using System.Collections.Generic;

namespace InDoOut_Core.Entities.Functions
{
    /// <summary>
    /// The current operating state of a <see cref="IFunction"/>
    /// </summary>
    public enum State
    {
        /// <summary>
        /// The entity is in an unknown state and hasn't been initialised properly.
        /// </summary>
        Unknown,
        /// <summary>
        /// The entity is being placed from within an editor.
        /// </summary>
        Placing,
        /// <summary>
        /// The entity is currently disabled and will not trigger.
        /// </summary>
        Disabled,
        /// <summary>
        /// The entity is currently waiting to be triggered.
        /// </summary>
        Waiting,
        /// <summary>
        /// The entity has been triggered and is currently processing.
        /// </summary>
        Processing,
        /// <summary>
        /// The entity has failed in some way and is in an error state.
        /// </summary>
        InError
    }

    /// <summary>
    /// Represents a function block containing <see cref="IInput"/>s and 
    /// <see cref="IOutput"/>s. This can be triggered with a <see cref="IInput"/>,
    /// </summary>
    public interface IFunction : INamedEntity, ITriggerable<IInput>, IConnectable<IOutput>
    {
        /// <summary>
        /// The current state of the function. See <see cref="State"/> for more
        /// details on the states that this function can enter.
        /// </summary>
        /// <seealso cref="State"/>
        State State { get; }

        /// <summary>
        /// Stop has been requested on the function, and it should be terminated as soon
        /// as possible.
        /// </summary>
        bool StopRequested { get; }

        /// <summary>
        /// All inputs that this function has.
        /// </summary>
        List<IInput> Inputs { get; }

        /// <summary>
        /// All outputs that this function has.
        /// </summary>
        List<IOutput> Outputs { get; }

        /// <summary>
        /// Politely asks for the function to be stopped, and waits for the user code to
        /// listen to the request. If the user code has no listener, it will continue
        /// regardless, and there's nothing that can be done about this.
        /// </summary>
        void PolitelyStop();
    }
}
