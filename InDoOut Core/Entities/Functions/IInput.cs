using InDoOut_Core.Entities.Core;

namespace InDoOut_Core.Entities.Functions
{
    /// <summary>
    /// Represents an input that is triggered by
    /// any <see cref="IOutput"/> entity.
    /// </summary>
    public interface IInput : INamedEntity, ITriggerable<IOutput>, IConnectable<IFunction>
    {
        /// <summary>
        /// The parent of this input.
        /// </summary>
        IFunction Parent { get; }
    }
}
