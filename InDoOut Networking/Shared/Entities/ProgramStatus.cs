using InDoOut_Core.Entities.Programs;
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
        public bool Running { get; set; } = false;

        [JsonProperty("stopping")]
        public bool Stopping { get; set; } = false;

        [JsonProperty("finishing")]
        public bool Finishing { get; set; } = false;

        [JsonProperty("name")]
        public string Name { get; set; } = null;

        [JsonProperty("lastTriggerTime")]
        public DateTime? LastTriggerTime { get; set; } = null;

        [JsonProperty("lastCompletionTime")]
        public DateTime? LastCompletionTime { get; set; } = null;

        [JsonProperty("id")]
        public Guid Id { get; set; } = Guid.Empty;

        [JsonProperty("functionStatus")]
        public FunctionStatus[] Functions { get; set; } = null;

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

            return new ProgramStatus()
            {
                Id = program.Id,
                Name = program.Name,
                Running = program.Running,
                Stopping = program.Stopping,
                Finishing = program.Finishing,
                LastCompletionTime = program.LastCompletionTime,
                LastTriggerTime = program.LastTriggerTime,
                Functions = functions
            };
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
