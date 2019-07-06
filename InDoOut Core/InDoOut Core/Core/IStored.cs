using System;

namespace InDoOut_Core.Core
{
    /// <summary>
    /// Represents stored data. Each entity has a unique <see cref="Guid"/>
    /// to identify it when saving and loading.
    /// </summary>
    interface IStored
    {
        /// <summary>
        /// The unique ID of this entity.
        /// </summary>
        Guid Id { get; }
    }
}
