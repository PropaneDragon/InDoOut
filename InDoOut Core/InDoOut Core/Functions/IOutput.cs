using InDoOut_Core.Core;

namespace InDoOut_Core.Functions
{
    /// <summary>
    /// Represents an output that can be connected to any
    /// <see cref="IInput"/> entity and is triggered by a
    /// <see cref="IFunction"/> entity.
    /// </summary>
    interface IOutput : INamedEntity, ITriggerable<IFunction>
    {
    }
}
