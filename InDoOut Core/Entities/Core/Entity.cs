using System;

namespace InDoOut_Core.Entities.Core
{
    /// <summary>
    /// A base, saveable entity. The root of all other entities.
    /// </summary>
    /// <seealso cref="IEntity"/>
    public abstract class Entity : IEntity
    {
        /// <summary>
        /// The unique ID of this entity.
        /// </summary>
        public Guid Id { get; protected set; } = Guid.NewGuid();
    }
}
