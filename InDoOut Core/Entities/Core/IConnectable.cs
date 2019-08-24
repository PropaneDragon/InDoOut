using System.Collections.Generic;

namespace InDoOut_Core.Entities.Core
{
    /// <summary>
    /// Represents an object that can be connected to an entity.
    /// </summary>
    public interface IConnectable
    {
        /// <summary>
        /// Check whether or not a <see cref="IEntity"/> can be connected to this
        /// object.
        /// </summary>
        /// <param name="entity">The entity to check.</param>
        /// <returns>Whether or not the given <see cref="IEntity"/> can be connected.</returns>
        bool CanAcceptConnection(IEntity entity);

        /// <summary>
        /// The connections this object has.
        /// </summary>
        List<ITriggerable> RawConnections { get; }
    }

    /// <summary>
    /// Represents an object that can be connected to an entity.
    /// </summary>
    public interface IConnectable<ConnectsToType> : IConnectable where ConnectsToType : class, ITriggerable
    {
        /// <summary>
        /// The connections this object has.
        /// </summary>
        List<ConnectsToType> Connections { get; }
    }
}
