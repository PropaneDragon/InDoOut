using InDoOut_Core.Basic;
using InDoOut_Core.Entities.Core;
using InDoOut_Core.Entities.Functions;
using InDoOut_Core.Extensions.Time;
using InDoOut_Networking.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace InDoOut_Networking.Entities
{
    public class NetworkedResult : INetworkedResult
    {
        private readonly Value _value = new Value();

        public bool Running { get; private set; } = false;
        public bool Finishing { get; private set; } = false;
        public bool IsSet => !string.IsNullOrEmpty(_value.RawValue);
        public bool ValidValue => _value.ValidValue;

        public string Name { get; private set; } = null;
        public string Description { get; private set; } = null;
        public string VariableName { get; set; } = null;
        public string RawValue { get; set; } = null;

        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime LastUpdateTime { get; set; } = DateTime.MinValue;
        public DateTime LastTriggerTime { get; private set; } = DateTime.MinValue;
        public DateTime LastCompletionTime { get; private set; } = DateTime.MinValue;

        public Dictionary<string, string> Metadata { get; private set; } = new Dictionary<string, string>();

        public List<IProperty> Connections { get; private set; } =  new List<IProperty>();
        public List<ITriggerable> RawConnections => Connections.Cast<ITriggerable>().ToList();

        public event EventHandler<ValueChangedEvent> OnValueChanged;

        public NetworkedResult()
        {
            _value.OnValueChanged += Value_OnValueChanged;
        }

        public bool UpdateFromStatus(ResultStatus status)
        {
            if (status != null)
            {
                var propertyExtractor = new PropertyExtractor<ResultStatus, NetworkedResult>(status);
                var convertedAll = UpdateMetadataFromStatus(status);
                return propertyExtractor.ApplyTo(this) && convertedAll;
            }

            return false;
        }

        public bool Connect(IProperty property)
        {
            if (!Connections.Contains(property))
            {
                Connections.Add(property);
            }

            return true;
        }

        public bool CanAcceptConnection(IEntity entity) => false;
        public bool CanBeTriggered(IEntity entity) => false;
        public bool Disconnect(IProperty input) => false;
        public bool HasBeenTriggeredSince(DateTime time) => LastTriggerTime.HasOccurredSince(time);
        public bool HasBeenTriggeredWithin(TimeSpan time) => LastTriggerTime.HasOccurredWithin(time, LastUpdateTime);
        public bool HasCompletedSince(DateTime time) => LastCompletionTime.HasOccurredSince(time);
        public bool HasCompletedWithin(TimeSpan time) => LastCompletionTime.HasOccurredWithin(time, LastUpdateTime);
        public void Trigger(IFunction triggeredBy) { }
        public T ValueAs<T>(T defaultValue = default) => _value.ValueAs(defaultValue);
        public bool ValueFrom<T>(T value) => _value.ValueFrom(value);
        public string ValueOrDefault(string defaultValue = "") => RawValue ?? defaultValue;

        private void Value_OnValueChanged(object sender, ValueChangedEvent e) => OnValueChanged?.Invoke(this, e);

        private bool UpdateMetadataFromStatus(ResultStatus status)
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
