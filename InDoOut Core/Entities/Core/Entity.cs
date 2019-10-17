using System;
using System.Collections.Generic;

namespace InDoOut_Core.Entities.Core
{
    /// <summary>
    /// A base, saveable entity. The root of all other entities.
    /// </summary>
    /// <seealso cref="IEntity"/>
    public abstract class NamedEntity : IEntity
    {
        /// <summary>
        /// The unique ID of this entity.
        /// </summary>
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Metadata associated with the entity.
        /// </summary>
        public Dictionary<string, string> Metadata { get; } = new Dictionary<string, string>();

        /// <summary>
        /// Gets the string representation of this entity.
        /// </summary>
        /// <returns>The string representation of this entity.</returns>
        public override string ToString()
        {
            return $"[{GetType()}] ({Id})";
        }
    }
}
