using InDoOut_Core.Entities.Functions;
using Newtonsoft.Json;
using System;

namespace InDoOut_Networking.Shared.Entities
{
    [JsonObject("functionStatus")]
    public class FunctionStatus
    {
        [JsonProperty("id")]
        public Guid Id { get; set; } = Guid.Empty;

        [JsonProperty("lastTriggerTime")]
        public DateTime? LastTriggerTime { get; set; } = null;

        [JsonProperty("lastCompletionTime")]
        public DateTime? LastCompletionTime { get; set; } = null;

        [JsonProperty("state")]
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
            if (function != null)
            {
                var status = new FunctionStatus()
                {
                    Id = function.Id,
                    LastTriggerTime = function.LastTriggerTime,
                    LastCompletionTime = function.LastCompletionTime,
                    State = function.State
                };

                return status;
            }

            return null;
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
