using InDoOut_Core.Basic;
using InDoOut_Core.Entities.Core;
using InDoOut_Core.Variables;
using System.Collections.Generic;

namespace InDoOut_Core.Entities.Functions
{
    /// <summary>
    /// The current operating state of a <see cref="IFunction"/>
    /// </summary>
    public enum State
    {
        /// <summary>
        /// The function is in an unknown state and hasn't been initialised properly.
        /// </summary>
        Unknown,
        /// <summary>
        /// The function is being placed from within an editor.
        /// </summary>
        Placing,
        /// <summary>
        /// The function is currently disabled and will not trigger.
        /// </summary>
        Disabled,
        /// <summary>
        /// The function is currently waiting to be triggered.
        /// </summary>
        Waiting,
        /// <summary>
        /// The function has been triggered and is currently processing.
        /// </summary>
        Processing,
        /// <summary>
        /// The function has been requested to stop and is stopping.
        /// </summary>
        Stopping,
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
        /// A thread safe, exception safe version of <see cref="INamed.Name"/>.
        /// </summary>
        string SafeName { get; }

        /// <summary>
        /// A thread safe, exception safe version of <see cref="Description"/>.
        /// </summary>
        string SafeDescription { get; }

        /// <summary>
        /// A thread safe, exception safe version of <see cref="Group"/>.
        /// </summary>
        string SafeGroup { get; }

        /// <summary>
        /// A thread safe, exception safe version of <see cref="Keywords"/>.
        /// </summary>
        string[] SafeKeywords { get; }

        /// <summary>
        /// The description of what the function does. This will be how it is seen by the user.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// What group this function belongs to. This will allow for it to be categorised into
        /// similar groups so they can be filtered easily.
        /// </summary>
        string Group { get; }

        /// <summary>
        /// Keywords associated with this function. This allows for it to be searched for by similar
        /// names.
        /// </summary>
        string[] Keywords { get; }

        /// <summary>
        /// All inputs that this function has.
        /// </summary>
        List<IInput> Inputs { get; }

        /// <summary>
        /// All outputs that this function has.
        /// </summary>
        List<IOutput> Outputs { get; }

        /// <summary>
        /// All properties that this function accepts.
        /// </summary>
        List<IProperty> Properties { get; }

        /// <summary>
        /// All results that this function gives.
        /// </summary>
        List<IResult> Results { get; }

        /// <summary>
        /// A shared variable store between multiple functions.
        /// </summary>
        /// <seealso cref="IVariableStore"/>
        IVariableStore VariableStore { get; set; }

        /// <summary>
        /// Politely asks for the function to be stopped, and waits for the user code to
        /// listen to the request. If the user code has no listener, it will continue
        /// regardless, and there's nothing that can be done about this.
        /// </summary>
        void PolitelyStop();
    }
}
