using InDoOut_Core.Entities.Core;
using InDoOut_Core.Entities.Functions;
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

        public DateTime LastTriggerTime { get; private set; } = DateTime.MinValue;
        public DateTime LastCompletionTime { get; private set; } = DateTime.MinValue;

        public Dictionary<string, string> Metadata => new Dictionary<string, string>();

        public List<IFunction> Connections => new List<IFunction>() { Parent };
        public List<ITriggerable> RawConnections => Connections.Cast<ITriggerable>().ToList();

        public IFunction Parent { get; private set; } = null;

        public void Trigger(IOutput triggeredBy) { }

        public bool UpdateFromStatus(InputStatus status)
        {
            if (status != null)
            {
                var propertyExtractor = new PropertyExtractor<InputStatus, NetworkedInput>(status);
                return propertyExtractor.ApplyTo(this);
            }

            return false;
        }

        public bool CanAcceptConnection(IEntity entity) => false;
        public bool CanBeTriggered(IEntity entity) => false;
        public bool HasBeenTriggeredSince(DateTime time) => false; //Todo
        public bool HasBeenTriggeredWithin(TimeSpan time) => false; //Todo
        public bool HasCompletedSince(DateTime time) => false; //Todo
        public bool HasCompletedWithin(TimeSpan time) => false; //Todo
    }
}
