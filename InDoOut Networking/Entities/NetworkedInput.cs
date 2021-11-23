using InDoOut_Core.Entities.Core;
using InDoOut_Core.Entities.Functions;
using InDoOut_Core.Extensions.Time;
using InDoOut_Networking.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace InDoOut_Networking.Entities
{
    public class NetworkedInput : INetworkedInput
    {
        public bool Running { get; private set; } = false;
        public bool Finishing { get; private set; } = false;

        public string Name { get; private set; } = null;

        public Guid Id { get; set; } = Guid.NewGuid();

        public DateTime LastUpdateTime { get; set; } = DateTime.MinValue;
        public DateTime LastTriggerTime { get; private set; } = DateTime.MinValue;
        public DateTime LastCompletionTime { get; private set; } = DateTime.MinValue;

        public Dictionary<string, string> Metadata { get; private set; } = new Dictionary<string, string>();

        public List<IFunction> Connections => new() { Parent };
        public List<ITriggerable> RawConnections => Connections.Cast<ITriggerable>().ToList();

        public IFunction Parent { get; private set; } = null;

        public NetworkedInput() { }

        public NetworkedInput(IFunction parent) : this()
        {
            Parent = parent;
        }

        public void Trigger(IOutput triggeredBy) { }

        public bool UpdateFromStatus(InputStatus status)
        {
            if (status != null)
            {
                var propertyExtractor = new PropertyExtractor<InputStatus, NetworkedInput>(status);
                var convertedAll = UpdateMetadataFromStatus(status);
                return propertyExtractor.ApplyTo(this) && convertedAll;
            }

            return false;
        }

        public bool CanAcceptConnection(IEntity entity) => false;
        public bool CanBeTriggered(IEntity entity) => false;
        public bool HasBeenTriggeredSince(DateTime time) => LastTriggerTime.HasOccurredSince(time);
        public bool HasBeenTriggeredWithin(TimeSpan time) => LastTriggerTime.HasOccurredWithin(time, LastUpdateTime);
        public bool HasCompletedSince(DateTime time) => LastCompletionTime.HasOccurredSince(time);
        public bool HasCompletedWithin(TimeSpan time) => LastCompletionTime.HasOccurredWithin(time, LastUpdateTime);

        private bool UpdateMetadataFromStatus(InputStatus status)
        {
            if (status != null)
            {
                Metadata.Clear();

                foreach (var pair in status.Metadata)
                {
                    Metadata.Add(pair.Key, pair.Value);
                }

                return true;
            }

            return false;
        }
    }
}
