﻿using InDoOut_Core.Entities.Core;
using InDoOut_Core.Entities.Functions;
using InDoOut_Core.Threading.Safety;
using InDoOut_Networking.Shared.Entities;
using System;
using System.Collections.Generic;

namespace InDoOut_Networking.Entities
{
    public class NetworkedFunction : INetworkedFunction
    {
        public bool StopRequested => State == State.Stopping;
        public bool Running => State == State.Processing;
        public bool Finishing => State == State.Completing;

        public string Name { get; private set; } = null;
        public string Description { get; private set; } = null;
        public string Group => null;
        public string SafeName => TryGet.ValueOrDefault(() => Name);
        public string SafeDescription => TryGet.ValueOrDefault(() => SafeDescription);
        public string SafeGroup => TryGet.ValueOrDefault(() => SafeGroup);

        public string[] Keywords { get; private set; } = new string[] { };
        public string[] SafeKeywords { get; private set; } = new string[] { };

        public State State { get; private set; } = State.Unknown;
        public IOutput TriggerOnFailure => null;

        public Guid Id { get; set; } = Guid.NewGuid();

        public DateTime LastTriggerTime { get; private set; } = DateTime.MinValue;
        public DateTime LastCompletionTime { get; private set; } = DateTime.MinValue;

        public List<IInput> Inputs { get; private set; } = new List<IInput>();
        public List<IOutput> Outputs { get; private set; } = new List<IOutput>();
        public List<IProperty> Properties { get; private set; } = new List<IProperty>();
        public List<IResult> Results { get; private set; } = new List<IResult>();
        public List<IOutput> Connections { get; private set; } = new List<IOutput>();
        public List<ITriggerable> RawConnections { get; private set; } = new List<ITriggerable>();

        public Dictionary<string, string> Metadata { get; private set; } = new Dictionary<string, string>();

        public NetworkedFunction() { }

        public void PolitelyStop() { } //Todo synchronise with networked entity

        public bool UpdateFromStatus(FunctionStatus status)
        {
            if (status != null)
            {
                var propertyExtractor = new PropertyExtractor<FunctionStatus, NetworkedFunction>(status);
                return propertyExtractor.ApplyTo(this);
            }

            return false;
        }

        public bool CanAcceptConnection(IEntity entity) => false;
        public bool CanBeTriggered(IEntity entity) => false;
        public bool HasBeenTriggeredSince(DateTime time) => false; //Todo link up with synchronised times
        public bool HasBeenTriggeredWithin(TimeSpan time) => false; //Todo link up with synchronised times
        public bool HasCompletedSince(DateTime time) => false; //Todo link up with synchronised times
        public bool HasCompletedWithin(TimeSpan time) => false; //Todo link up with synchronised times
        public void Trigger(IInput triggeredBy) { } //Todo synchronise with networked entity
    }
}
