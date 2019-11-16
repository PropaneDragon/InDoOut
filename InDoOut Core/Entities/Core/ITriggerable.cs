using System;

namespace InDoOut_Core.Entities.Core
{
    /// <summary>
    /// Represents an entity that can be triggered by another entity.
    /// </summary>
    public interface ITriggerable
    {
        /// <summary>
        /// The current running state of the object.
        /// </summary>
        bool Running { get; }

        /// <summary>
        /// Represents a state where the entity is technically still running, but is in a state
        /// where it is coming to a finish and can be triggered again regardless.
        /// </summary>
        bool Finishing { get; }

        /// <summary>
        /// The time this entity was last triggered.
        /// </summary>
        DateTime LastTriggerTime { get; }

        /// <summary>
        /// Check whether or not the given <see cref="IEntity"/> can trigger this object.
        /// </summary>
        /// <param name="entity">The entity to check.</param>
        /// <returns>Whether this object can be triggered by the given <see cref="IEntity"/>.</returns>
        bool CanBeTriggered(IEntity entity);

        /// <summary>
        /// Checks whether the entity has been triggered since the given <paramref name="time"/>.
        /// </summary>
        /// <param name="time">The time to check.</param>
        /// <returns>Whether the entity has been triggered since the given time.</returns>
        bool HasBeenTriggeredSince(DateTime time);

        /// <summary>
        /// Checks whether the entity has been triggered within the given <paramref name="time"/>. Passing a time
        /// of 5 seconds will return whether the entity has been triggered within the last 5 seconds.
        /// </summary>
        /// <param name="time">The time to check.</param>
        /// <returns>Whether the entity has been triggered within the given time.</returns>
        bool HasBeenTriggeredWithin(TimeSpan time);
    }

    /// <summary>
    /// Represents an entity that can be triggered by another entity.
    /// </summary>
    public interface ITriggerable<TriggerType> : ITriggerable where TriggerType : class, IEntity
    {
        /// <summary>
        /// Triggers this entity using the given <typeparamref name="TriggerType"/>
        /// </summary>
        /// <param name="triggeredBy">The entity to trigger this object.</param>
        void Trigger(TriggerType triggeredBy);
    }
}
