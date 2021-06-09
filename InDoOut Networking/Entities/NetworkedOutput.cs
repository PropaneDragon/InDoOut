using InDoOut_Core.Entities.Core;
using InDoOut_Core.Entities.Functions;
using InDoOut_Networking.Shared.Entities;
using System;
using System.Collections.Generic;

namespace InDoOut_Networking.Entities
{
    public class NetworkedOutput : INetworkedOutput
    {
        public bool Running { get; private set; } = false;
        public bool Finishing { get; private set; } = false;

        public string Name { get; private set; } = null;

        public Guid Id { get; set; } = Guid.NewGuid();

        public DateTime LastTriggerTime { get; private set; } = DateTime.MinValue;
        public DateTime LastCompletionTime { get; private set; } = DateTime.MinValue;

        public Dictionary<string, string> Metadata => new Dictionary<string, string>();

        public List<IInput> Connections => new List<IInput>();
        public List<ITriggerable> RawConnections => new List<ITriggerable>();

        public void Trigger(IFunction triggeredBy) { }

        public bool UpdateFromStatus(OutputStatus status)
        {
            if (status != null)
            {

            }

            return false;
        }

        public bool Connect(IInput input) => false;
        public bool Disconnect(IInput input) => false;
        public bool CanAcceptConnection(IEntity entity) => false;
        public bool CanBeTriggered(IEntity entity) => false;
        public bool HasBeenTriggeredSince(DateTime time) => false; //Todo
        public bool HasBeenTriggeredWithin(TimeSpan time) => false; //Todo
        public bool HasCompletedSince(DateTime time) => false; //Todo
        public bool HasCompletedWithin(TimeSpan time) => false; //Todo
    }
}
