using InDoOut_Core.Entities.Core;
using InDoOut_Core.Entities.Functions;
using InDoOut_Networking.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace InDoOut_Networking.Entities
{
    public class NetworkedFunction : INetworkedFunction
    {
        private readonly IFunction _internalReferenceFunction = null;

        public bool StopRequested => false; //Todo synchronisation?
        public bool Running { get; private set; }
        public bool Finishing { get; private set; }

        public string Name => _internalReferenceFunction.Name;
        public string Description => _internalReferenceFunction?.Description;
        public string Group => _internalReferenceFunction?.Group;
        public string SafeName => _internalReferenceFunction?.SafeName;
        public string SafeDescription => _internalReferenceFunction?.SafeDescription;
        public string SafeGroup => _internalReferenceFunction?.SafeGroup;

        public string[] Keywords => _internalReferenceFunction?.Keywords;
        public string[] SafeKeywords => _internalReferenceFunction?.SafeKeywords;

        public State State { get; private set; }
        public IOutput TriggerOnFailure => null;

        public Guid Id { get => _internalReferenceFunction.Id; set => _internalReferenceFunction.Id = value; }

        public DateTime LastTriggerTime { get; private set; }
        public DateTime LastCompletionTime { get; private set; }

        public List<IInput> Inputs => _internalReferenceFunction.Inputs;
        public List<IOutput> Outputs => _internalReferenceFunction.Outputs;
        public List<IProperty> Properties => _internalReferenceFunction.Properties;
        public List<IResult> Results => _internalReferenceFunction.Results;
        public List<IOutput> Connections => _internalReferenceFunction.Connections;
        public List<ITriggerable> RawConnections => _internalReferenceFunction.RawConnections;

        public Dictionary<string, string> Metadata => _internalReferenceFunction.Metadata;

        private NetworkedFunction() { }

        public NetworkedFunction(IFunction function) : this()
        {
            _internalReferenceFunction = function;
        }

        public static NetworkedFunction CreateFromFunction(IFunction function)
        {
            var networkedFunction = new NetworkedFunction(function);

            return networkedFunction;
        }

        public void PolitelyStop() { } //Todo synchronise with networked entity

        public bool CanAcceptConnection(IEntity entity) => _internalReferenceFunction.CanAcceptConnection(entity);
        public bool CanBeTriggered(IEntity entity) => _internalReferenceFunction.CanBeTriggered(entity);
        public bool HasBeenTriggeredSince(DateTime time) => false; //Todo link up with synchronised times
        public bool HasBeenTriggeredWithin(TimeSpan time) => false; //Todo link up with synchronised times
        public bool HasCompletedSince(DateTime time) => false; //Todo link up with synchronised times
        public bool HasCompletedWithin(TimeSpan time) => false; //Todo link up with synchronised times
        public void Trigger(IInput triggeredBy) { } //Todo synchronise with networked entity

        public bool UpdateFromStatus(ProgramStatus status)
        {
            if (status != null)
            {
                var foundFunction = status.Functions.FirstOrDefault(function => function.Id == Id);
                if (foundFunction != null)
                {
                    LastTriggerTime = foundFunction.LastTriggerTime ?? DateTime.MinValue;
                    LastCompletionTime = foundFunction.LastCompletionTime ?? DateTime.MinValue;
                    State = foundFunction.State;

                    return true;
                }
            }

            return false;
        }
    }
}
