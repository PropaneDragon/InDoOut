using System;
using System.Collections.Generic;

namespace InDoOut_Core.Basic
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
        Guid Id { get; set; }

        /// <summary>
        /// Metadata associated with the stored data.
        /// </summary>
        Dictionary<string, string> Metadata { get; }
    }
}
