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

        public DateTime LastUpdateTime { get; set; } = DateTime.MinValue;
        public DateTime LastTriggerTime { get; private set; } = DateTime.MinValue;
        public DateTime LastCompletionTime { get; private set; } = DateTime.MinValue;

        public List<IInput> Inputs { get; private set; } = new List<IInput>();
        public List<IOutput> Outputs { get; private set; } = new List<IOutput>();
        public List<IProperty> Properties { get; private set; } = new List<IProperty>();
        public List<IResult> Results { get; private set; } = new List<IResult>();
        public List<IOutput> Connections => Outputs;
        public List<ITriggerable> RawConnections => Outputs.Cast<ITriggerable>().ToList();

        public Dictionary<string, string> Metadata { get; private set; } = new Dictionary<string, string>();

        public NetworkedFunction() { }

        public void PolitelyStop() { } //Todo synchronise with networked entity

        public bool UpdateFromStatus(FunctionStatus status)
        {
            if (status != null)
            {
                var convertedAll = true;
                var propertyExtractor = new PropertyExtractor<FunctionStatus, NetworkedFunction>(status);

                convertedAll = UpdateMetadataFromStatus(status) && convertedAll;
                convertedAll = UpdateInputsFromStatus(status) && convertedAll;
                convertedAll = UpdateOutputsFromStatus(status) && convertedAll;
                convertedAll = UpdatePropertiesFromStatus(status) && convertedAll;
                convertedAll = UpdateResultsFromStatus(status) && convertedAll;

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
        public void Trigger(IInput triggeredBy) { } //Todo synchronise with networked entity

        private bool UpdateMetadataFromStatus(FunctionStatus status)
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

        private bool UpdateResultsFromStatus(FunctionStatus status)
        {
            var convertedAll = true;

            foreach (var resultsStatus in status.Results)
            {
                var foundResult = Results.FirstOrDefault(result => result.Id == resultsStatus.Id);
                if (foundResult is INetworkedResult networkedResult)
                {
                    convertedAll = networkedResult.UpdateFromStatus(resultsStatus) && convertedAll;
                }
                else
                {
                    var resultToAdd = new NetworkedResult();
                    convertedAll = resultToAdd.UpdateFromStatus(resultsStatus) && convertedAll;

                    Results.Add(resultToAdd);
                }
            }

            return convertedAll;
        }

        private bool UpdatePropertiesFromStatus(FunctionStatus status)
        {
            var convertedAll = true;

            foreach (var propertyStatus in status.Properties)
            {
                var foundProperty = Properties.FirstOrDefault(property => property.Id == propertyStatus.Id);
                if (foundProperty is INetworkedProperty networkedProperty)
                {
                    convertedAll = networkedProperty.UpdateFromStatus(propertyStatus) && convertedAll;
                }
                else
                {
                    var propertyToAdd = new NetworkedProperty(this);
                    convertedAll = propertyToAdd.UpdateFromStatus(propertyStatus) && convertedAll;

                    Properties.Add(propertyToAdd);
                }
            }

            return convertedAll;
        }

        private bool UpdateOutputsFromStatus(FunctionStatus status)
        {
            var convertedAll = true;

            foreach (var outputStatus in status.Outputs)
            {
                var foundOutput = Outputs.FirstOrDefault(output => output.Id == outputStatus.Id);
                if (foundOutput is INetworkedOutput networkedOutput)
                {
                    convertedAll = networkedOutput.UpdateFromStatus(outputStatus) && convertedAll;
                }
                else
                {
                    var outputToAdd = new NetworkedOutput();
                    convertedAll = outputToAdd.UpdateFromStatus(outputStatus) && convertedAll;

                    Outputs.Add(outputToAdd);
                }
            }

            return convertedAll;
        }

        private bool UpdateInputsFromStatus(FunctionStatus status)
        {
            var convertedAll = true;

            foreach (var inputStatus in status.Inputs)
            {
                var foundInput = Inputs.FirstOrDefault(input => input.Id == inputStatus.Id);
                if (foundInput is INetworkedInput networkedInput)
                {
                    convertedAll = networkedInput.UpdateFromStatus(inputStatus) && convertedAll;
                }
                else
                {
                    var inputToAdd = new NetworkedInput(this);
                    convertedAll = inputToAdd.UpdateFromStatus(inputStatus) && convertedAll;

                    Inputs.Add(inputToAdd);
                }
            }

            return convertedAll;
        }
    }
}
