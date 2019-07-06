using InDoOut_Core.Core;
using System.Collections.Generic;

namespace InDoOut_Core.Functions
{
    /// <summary>
    /// The current operating state of a <see cref="IFunction"/>
    /// </summary>
    enum State
    {
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
    interface IFunction : INamedEntity, ITriggerable<IInput>
    {
        /// <summary>
        /// The current state of the function. See <see cref="State"/> for more
        /// details on the states that this function can enter.
        /// </summary>
        /// <seealso cref="State"/>
        State State { get; }

        /// <summary>
        /// All inputs that this function has.
        /// </summary>
        List<IInput> Inputs { get; }

        /// <summary>
        /// All outputs that this function has.
        /// </summary>
        List<IOutput> Outputs { get; }
    }
}
