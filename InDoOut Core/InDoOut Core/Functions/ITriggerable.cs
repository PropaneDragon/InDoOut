using InDoOut_Core.Core;

namespace InDoOut_Core.Functions
{
    /// <summary>
    /// Represents an entity that can be triggered by another entity.
    /// </summary>
    interface ITriggerable
    {
        bool CanTrigger(IEntity entity);
    }

    interface ITriggerable<TriggerType> : ITriggerable where TriggerType : class, IEntity
    {
        void Trigger(TriggerType triggeredBy);
    }
}
