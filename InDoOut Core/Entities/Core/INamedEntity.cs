using InDoOut_Core.Basic;

namespace InDoOut_Core.Entities.Core
{
    /// <summary>
    /// Represents an <see cref="IEntity"/> that can have a name.
    /// </summary>
    public interface INamedEntity : IEntity, INamed
    {
    }
}
