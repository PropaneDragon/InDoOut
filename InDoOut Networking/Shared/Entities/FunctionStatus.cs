using InDoOut_Core.Entities.Functions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace InDoOut_Networking.Shared.Entities
{
    [JsonObject("functionStatus")]
    public class FunctionStatus
    {
        [JsonProperty("creationTime")]
        [ExtractProperty("LastUpdateTime", true)]
        public DateTime CreationTime { get; set; } = DateTime.Now;

        [JsonProperty("name")]
        [ExtractProperty("Name")]
        public string Name { get; set; } = null;

        [JsonProperty("id")]
        [ExtractProperty("Id")]
        public Guid Id { get; set; } = Guid.Empty;

        [JsonProperty("lastTriggerTime")]
        [ExtractProperty("LastTriggerTime")]
        public DateTime LastTriggerTime { get; set; } = DateTime.MinValue;

        [JsonProperty("lastCompletionTime")]
        [ExtractProperty("LastCompletionTime")]
        public DateTime LastCompletionTime { get; set; } = DateTime.MinValue;

        [JsonProperty("state")]
        [ExtractProperty("State")]
        public State State { get; set; } = State.Unknown;

        [JsonProperty("inputStatus")]
        public InputStatus[] Inputs { get; set; } = new InputStatus[] { };

        [JsonProperty("outputStatus")]
        public OutputStatus[] Outputs { get; set; } = new OutputStatus[] { };

        [JsonProperty("propertyStatus")]
        public PropertyStatus[] Properties { get; set; } = new PropertyStatus[] { };

        [JsonProperty("resultStatus")]
        public ResultStatus[] Results { get; set; } = new ResultStatus[] { };

        [JsonProperty("metadata")]
        public Dictionary<string, string> Metadata { get; set; } = new Dictionary<string, string>();

        public static FunctionStatus FromJson(string json)
        {
            try
            {
                var generatedStatus = JsonConvert.DeserializeObject<FunctionStatus>(json);
                return generatedStatus;
            }
            catch { }

            return null;
        }

        public static FunctionStatus FromFunction(IFunction function)
        {
            var inputs = function.Inputs.Select(input => InputStatus.FromInput(input)).Where(result => result != null).ToArray();
            var outputs = function.Outputs.Select(output => OutputStatus.FromOutput(output)).Where(result => result != null).ToArray();
            var properties = function.Properties.Select(property => PropertyStatus.FromProperty(property)).Where(result => result != null).ToArray();
            var results = function.Results.Select(result => ResultStatus.FromResult(result)).Where(result => result != null).ToArray();
            var metadata = new Dictionary<string, string>(function.Metadata);
            var status = new FunctionStatus() { Inputs = inputs, Outputs = outputs, Properties = properties, Results = results, Metadata = metadata };
            var propertyExtractor = new PropertyExtractor<FunctionStatus, IFunction>(status);

            return propertyExtractor.ExtractFrom(function) ? status : null;
        }

        public string ToJson()
        {
            try
            {
                var generatedText = JsonConvert.SerializeObject(this);
                return generatedText;
            }
            catch { }

            return null;
        }
    }
}
