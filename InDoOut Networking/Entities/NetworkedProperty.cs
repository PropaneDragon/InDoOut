using InDoOut_Core.Basic;
using InDoOut_Core.Entities.Core;
using InDoOut_Core.Entities.Functions;
using InDoOut_Core.Extensions.Time;
using InDoOut_Core.Threading.Safety;
using InDoOut_Networking.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace InDoOut_Networking.Entities
{
    public class NetworkedProperty : INetworkedProperty
    {
        private readonly Value _value = new();

        public bool Required { get; private set; }
        public bool Running { get; private set; } = false;
        public bool Finishing { get; private set; } = false;
        public bool ValidValue => _value.ValidValue;

        public string Name { get; private set; } = null;
        public string Description { get; private set; } = null;
        public string SafeDescription => TryGet.ValueOrDefault(() => Description);
        public string RawValue { get => _value.RawValue; set => _value.RawValue = value; }
        public string RawComputedValue { get => RawValue; set => RawValue = value; }
        public string LastSetValue { get => RawValue; set => RawValue = value; }

        public IFunction Parent { get; private set; } = null;

        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime LastUpdateTime { get; set; } = DateTime.MinValue;
        public DateTime LastTriggerTime { get; private set; } = DateTime.MinValue;
        public DateTime LastCompletionTime { get; private set; } = DateTime.MinValue;

        public Dictionary<string, string> Metadata { get; private set; } = new Dictionary<string, string>();

        public List<IFunction> Connections => new() { Parent };
        public List<ITriggerable> RawConnections => Connections.Cast<ITriggerable>().ToList();

        public event EventHandler<ValueChangedEvent> OnValueChanged;

        public NetworkedProperty()
        {
            _value.OnValueChanged += Value_OnValueChanged;
        }

        public NetworkedProperty(IFunction parent) : this()
        {
            Parent = parent;
        }

        public bool UpdateFromStatus(PropertyStatus status)
        {
            if (status != null)
            {
                var propertyExtractor = new PropertyExtractor<PropertyStatus, NetworkedProperty>(status);
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
        public bool Connect(IFunction function) => false;
        public void Trigger(IResult triggeredBy) { }
        public T ValueAs<T>(T defaultValue = default) => _value.ValueAs(defaultValue);
        public bool ValueFrom<T>(T value) => _value.ValueFrom(value);
        public string ValueOrDefault(string defaultValue = "") => RawValue ?? defaultValue;

        private void Value_OnValueChanged(object sender, ValueChangedEvent e) => OnValueChanged?.Invoke(this, e);

        private bool UpdateMetadataFromStatus(PropertyStatus status)
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
