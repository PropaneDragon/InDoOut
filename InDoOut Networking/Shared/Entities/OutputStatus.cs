using InDoOut_Core.Entities.Functions;
using Newtonsoft.Json;
using System;

namespace InDoOut_Networking.Shared.Entities
{
    [JsonObject("outputStatus")]
    public class OutputStatus
    {
        [JsonProperty("running")]
        [ExtractProperty("Running")]
        public bool Running { get; set; } = false;

        [JsonProperty("finishing")]
        [ExtractProperty("Finishing")]
        public bool Finishing { get; set; } = false;

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

        public static OutputStatus FromJson(string json)
        {
            try
            {
                var generatedStatus = JsonConvert.DeserializeObject<OutputStatus>(json);
                return generatedStatus;
            }
            catch { }

            return null;
        }

        public static OutputStatus FromOutput(IOutput output)
        {
            var status = new OutputStatus();
            var propertyExtractor = new PropertyExtractor<OutputStatus, IOutput>(status);

            return propertyExtractor.ExtractFrom(output) ? status : null;
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
