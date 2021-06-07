using InDoOut_Core.Entities.Functions;
using Newtonsoft.Json;
using System;

namespace InDoOut_Networking.Shared.Entities
{
    [JsonObject("functionStatus")]
    public class FunctionStatus
    {
        [JsonProperty("id")]
        [ExtractProperty("Id")]
        public Guid Id { get; set; } = Guid.Empty;

        [JsonProperty("lastTriggerTime")]
        [ExtractProperty("LastTriggerTime")]
        public DateTime? LastTriggerTime { get; set; } = null;

        [JsonProperty("lastCompletionTime")]
        [ExtractProperty("LastCompletionTime")]
        public DateTime? LastCompletionTime { get; set; } = null;

        [JsonProperty("state")]
        [ExtractProperty("State")]
        public State State { get; set; } = State.Unknown;

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
            var status = new FunctionStatus();
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
