using InDoOut_Core.Entities.Core;
using InDoOut_Core.Entities.Functions;
using InDoOut_Core.Time;
using InDoOut_Networking.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace InDoOut_Networking.Entities
{
    public class NetworkedOutput : INetworkedOutput
    {
        public bool Running { get; private set; } = false;
        public bool Finishing { get; private set; } = false;

        public string Name { get; private set; } = null;

        public Guid Id { get; set; } = Guid.NewGuid();

        public DateTime LastUpdateTime { get; set; } = DateTime.MinValue;
        public DateTime LastTriggerTime { get; private set; } = DateTime.MinValue;
        public DateTime LastCompletionTime { get; private set; } = DateTime.MinValue;

        public Dictionary<string, string> Metadata { get; private set; } = new Dictionary<string, string>();

        public List<IInput> Connections { get; private set; } = new List<IInput>();
        public List<ITriggerable> RawConnections => Connections.Cast<ITriggerable>().ToList();

        public void Trigger(IFunction triggeredBy) { }

        public bool UpdateFromStatus(OutputStatus status)
        {
            if (status != null)
            {
                var propertyExtractor = new PropertyExtractor<OutputStatus, NetworkedOutput>(status);
                var convertedAll = UpdateMetadataFromStatus(status);
                return propertyExtractor.ApplyTo(this) && convertedAll;
            }

            return false;
        }

        public bool Connect(IInput input)
        {
            if (!Connections.Contains(input))
            {
                Connections.Add(input);
            }

            return true;
        }

        public bool Disconnect(IInput input) => false;
        public bool CanAcceptConnection(IEntity entity) => false;
        public bool CanBeTriggered(IEntity entity) => false;
        public bool HasBeenTriggeredSince(DateTime time) => LastTriggerTime.HasOccurredSince(time);
        public bool HasBeenTriggeredWithin(TimeSpan time) => LastTriggerTime.HasOccurredWithin(time, LastUpdateTime);
        public bool HasCompletedSince(DateTime time) => LastCompletionTime.HasOccurredSince(time);
        public bool HasCompletedWithin(TimeSpan time) => LastCompletionTime.HasOccurredWithin(time, LastUpdateTime);

        private bool UpdateMetadataFromStatus(OutputStatus status)
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
