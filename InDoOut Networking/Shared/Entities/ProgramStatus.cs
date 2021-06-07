using InDoOut_Core.Entities.Programs;
using InDoOut_Networking.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Linq;

namespace InDoOut_Networking.Shared.Entities
{
    [JsonObject("programStatus")]
    public class ProgramStatus
    {
        [JsonProperty("running")]
        [ExtractProperty("Running")]
        public bool Running { get; set; } = false;

        [JsonProperty("stopping")]
        [ExtractProperty("Stopping")]
        public bool Stopping { get; set; } = false;

        [JsonProperty("finishing")]
        [ExtractProperty("Finishing")]
        public bool Finishing { get; set; } = false;

        [JsonProperty("name")]
        [ExtractProperty("Name")]
        public string Name { get; set; } = null;

        [JsonProperty("lastTriggerTime")]
        [ExtractProperty("LastTriggerTime")]
        public DateTime? LastTriggerTime { get; set; } = null;

        [JsonProperty("lastCompletionTime")]
        [ExtractProperty("LastCompletionTime")]
        public DateTime? LastCompletionTime { get; set; } = null;

        [JsonProperty("id")]
        [ExtractProperty("Id")]
        public Guid Id { get; set; } = Guid.Empty;

        [JsonProperty("functionStatus")]
        public FunctionStatus[] Functions { get; set; } = new FunctionStatus[] { };

        public static ProgramStatus FromJson(string json)
        {
            try
            {
                var generatedStatus = JsonConvert.DeserializeObject<ProgramStatus>(json);
                return generatedStatus;
            }
            catch { }

            return null;
        }

        public static ProgramStatus FromProgram(IProgram program)
        {
            var functions = program.Functions.Select(function => FunctionStatus.FromFunction(function)).ToArray();
            var status = new ProgramStatus() { Functions = functions };
            var propertyExtractor = new PropertyExtractor<ProgramStatus, IProgram>(status);

            return propertyExtractor.ExtractFrom(program) ? status : null;
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
