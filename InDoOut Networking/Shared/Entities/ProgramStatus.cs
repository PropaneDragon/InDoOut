using InDoOut_Core.Entities.Programs;
using InDoOut_Json_Storage;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
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

        [JsonProperty("functionStatus")]
        public FunctionStatus[] Functions { get; set; } = new FunctionStatus[] { };

        [JsonProperty("connectionStatus")]
        public ConnectionStatus[] Connections { get; set; } = new ConnectionStatus[] { };

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
            var connections = program.Functions.SelectMany(function => JsonConnection.CreateFromFunction(function).Select(jsonConnection => new ConnectionStatus(jsonConnection))).ToArray();
            var metadata = new Dictionary<string, string>(program.Metadata);
            var status = new ProgramStatus() { Functions = functions, Connections = connections, Metadata = metadata };
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
