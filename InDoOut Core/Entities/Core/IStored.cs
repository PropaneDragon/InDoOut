using System;

namespace InDoOut_Core.Entities.Core
{
    /// <summary>
    /// Represents stored data. Each entity has a unique <see cref="Guid"/>
    /// to identify it when saving and loading.
    /// </summary>
    public interface IStored
    {
        /// <summary>
        /// The unique ID of this entity.
        /// </summary>
        Guid Id { get; }
    }
}
