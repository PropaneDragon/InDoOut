using InDoOut_Core.Basic;

namespace InDoOut_Core.Variables
{
    /// <summary>
    /// Represents an individual variable. Variables have names and values which store state
    /// over multiple functions.
    /// </summary>
    public interface IVariable : IValue, INamed
    {
        /// <summary>
        /// Returns whether the variable is valid.
        /// </summary>
        bool Valid { get; }
    }
}
