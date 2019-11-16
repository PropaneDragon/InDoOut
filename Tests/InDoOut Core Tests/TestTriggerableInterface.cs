using System;
using InDoOut_Core.Entities.Core;

namespace InDoOut_Core_Tests
{
    internal class TestTriggerableInterface : ITriggerable<IEntity>
    {
        public bool Running { get; set; } = false;
        public bool Triggerable { get; set; } = true;
        public bool Triggered { get; set; } = false;

        public DateTime LastTriggerTime { get; set; } = DateTime.MinValue;

        public bool Finishing => false;

        public bool CanBeTriggered(IEntity entity)
        {
            return Triggerable;
        }

        public void Trigger(IEntity triggeredBy)
        {
            LastTriggerTime = DateTime.Now;

            Triggered = true;
        }

        public bool HasBeenTriggeredSince(DateTime time)
        {
            return LastTriggerTime >= time;
        }

        public bool HasBeenTriggeredWithin(TimeSpan time)
        {
            return LastTriggerTime >= DateTime.Now - time;
        }
    }
}
