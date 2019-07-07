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
        /// Check whether or not the given <see cref="IEntity"/> can trigger this object.
        /// </summary>
        /// <param name="entity">The entity to check.</param>
        /// <returns>Whether this object can be triggered by the given <see cref="IEntity"/>.</returns>
        bool CanBeTriggered(IEntity entity);
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
