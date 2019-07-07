using InDoOut_Core.Entities.Core;

namespace InDoOut_Core_Tests
{
    internal class TestTriggerableInterface : ITriggerable<IEntity>
    {
        public bool Running { get; set; } = false;
        public bool Triggerable { get; set; } = true;
        public bool Triggered { get; set; } = false;

        public bool CanBeTriggered(IEntity entity)
        {
            return Triggerable;
        }

        public void Trigger(IEntity triggeredBy)
        {
            Triggered = true;
        }
    }
}
