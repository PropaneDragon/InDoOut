using InDoOut_Core.Entities.Functions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace InDoOut_Networking.Shared.Entities
{
    [JsonObject("propertyStatus")]
    public class PropertyStatus
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

        [JsonProperty("value")]
        [ExtractProperty("RawComputedValue")]
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

        public static PropertyStatus FromJson(string json)
        {
            try
            {
                var generatedStatus = JsonConvert.DeserializeObject<PropertyStatus>(json);
                return generatedStatus;
            }
            catch { }

            return null;
        }

        public static PropertyStatus FromProperty(IProperty property)
        {
            var metadata = new Dictionary<string, string>(property.Metadata);
            var status = new PropertyStatus() { Metadata = metadata };
            var propertyExtractor = new PropertyExtractor<PropertyStatus, IProperty>(status);

            return propertyExtractor.ExtractFrom(property) ? status : null;
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
