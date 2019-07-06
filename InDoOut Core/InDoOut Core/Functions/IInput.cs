using InDoOut_Core.Core;

namespace InDoOut_Core.Functions
{
    /// <summary>
    /// Represents an input that is triggered by
    /// any <see cref="IOutput"/> entity.
    /// </summary>
    interface IInput : INamedEntity, ITriggerable<IOutput>
    {
    }
}
