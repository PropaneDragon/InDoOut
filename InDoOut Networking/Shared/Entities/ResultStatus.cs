using InDoOut_Core.Entities.Functions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace InDoOut_Networking.Shared.Entities
{
    [JsonObject("resultStatus")]
    public class ResultStatus
    {
        [JsonProperty("creationTime")]
        [ExtractProperty("LastUpdateTime", true)]
        public DateTime CreationTime { get; set; } = DateTime.Now;

        [JsonProperty("running")]
        [ExtractProperty("Running")]
        public bool Running { get; set; } = false;

        [JsonProperty("finishing")]
        [ExtractProperty("Finishing")]
        public bool Finishing { get; set; } = false;

        [JsonProperty("name")]
        [ExtractProperty("Name")]
        public string Name { get; set; } = null;

        [JsonProperty("description")]
        [ExtractProperty("Description")]
        public string Description { get; set; } = null;

        [JsonProperty("variableName")]
        [ExtractProperty("VariableName")]
        public string VariableName { get; set; } = null;

        [JsonProperty("value")]
        [ExtractProperty("RawValue")]
        public string Value { get; set; } = null;

        [JsonProperty("id")]
        [ExtractProperty("Id")]
        public Guid Id { get; set; } = Guid.Empty;

        [JsonProperty("lastTriggerTime")]
        [ExtractProperty("LastTriggerTime")]
        public DateTime LastTriggerTime { get; set; } = DateTime.MinValue;

        [JsonProperty("lastCompletionTime")]
        [ExtractProperty("LastCompletionTime")]
        public DateTime LastCompletionTime { get; set; } = DateTime.MinValue;

        [JsonProperty("metadata")]
        public Dictionary<string, string> Metadata { get; set; } = new Dictionary<string, string>();

        public static ResultStatus FromJson(string json)
        {
            try
            {
                var generatedStatus = JsonConvert.DeserializeObject<ResultStatus>(json);
                return generatedStatus;
            }
            catch { }

            return null;
        }

        public static ResultStatus FromResult(IResult result)
        {
            var metadata = new Dictionary<string, string>(result.Metadata);
            var status = new ResultStatus() { Metadata = metadata };
            var propertyExtractor = new PropertyExtractor<ResultStatus, IResult>(status);

            return propertyExtractor.ExtractFrom(result) ? status : null;
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
