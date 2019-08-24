using InDoOut_Core.Basic;
using InDoOut_Core.Entities.Core;
using InDoOut_Core.Entities.Functions;
using InDoOut_Core.Entities.Programs;
using InDoOut_Core.Functions;
using InDoOut_Plugins.Loaders;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace InDoOut_Json_Storage
{
    /// <summary>
    /// Represents a basic JSON shell for an <see cref="IProgram"/>.
    /// </summary>
    [JsonObject("program")]
    public class JsonProgram
    {
        /// <summary>
        /// Program Id.
        /// </summary>
        [JsonProperty("id")]
        public Guid Id { get; set; } = Guid.Empty;

        /// <summary>
        /// Program functions.
        /// </summary>
        [JsonProperty("functions")]
        public List<JsonFunction> Functions { get; set; } = new List<JsonFunction>();

        /// <summary>
        /// Program, function connections.
        /// </summary>
        [JsonProperty("connections")]
        public List<JsonConnection> Connections { get; set; } = new List<JsonConnection>();

        /// <summary>
        /// Values for function properties.
        /// </summary>
        [JsonProperty("propertyValues")]
        public List<JsonPropertyValue> PropertyValues { get; set; } = new List<JsonPropertyValue>();

        /// <summary>
        /// Program metadata.
        /// </summary>
        [JsonProperty("metadata")]
        public Dictionary<string, string> Metadata { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// Creates a <see cref="JsonProgram"/> shell for a given <paramref name="program"/>.
        /// </summary>
        /// <param name="program">The program to construct a JSON shell for.</param>
        /// <returns>A created JSON shell, or null if failed.</returns>
        public static JsonProgram CreateFromProgram(IProgram program)
        {
            if (program != null)
            {
                var jsonProgram = new JsonProgram()
                {
                    Id = program.Id,
                    Metadata = program.Metadata
                };

                foreach (var function in program.Functions)
                {
                    var jsonFunction = JsonFunction.CreateFromFunction(function);
                    if (jsonFunction != null)
                    {
                        jsonProgram.Functions.Add(jsonFunction);
                    }

                    var jsonConnections = JsonConnection.CreateFromFunction(function);
                    if (jsonConnections != null)
                    {
                        jsonProgram.Connections.AddRange(jsonConnections);
                    }

                    var jsonValues = JsonPropertyValue.CreateFromFunction(function);
                    if (jsonValues != null)
                    {
                        jsonProgram.PropertyValues.AddRange(jsonValues);
                    }
                }

                return jsonProgram;
            }

            return null;
        }

        /// <summary>
        /// Sets the data in a given <paramref name="program"/> from the stored data.
        /// </summary>
        /// <param name="program">The program to set.</param>
        /// <param name="builder">The function builder to generate saved functions.</param>
        /// <param name="loadedPlugins">The currently loaded plugins to search through.</param>
        /// <returns>Whether the data was set successfully.</returns>
        public bool Set(IProgram program, IFunctionBuilder builder, ILoadedPlugins loadedPlugins)
        {
            if (program != null && builder != null && loadedPlugins != null)
            {
                var functionIdMap = new Dictionary<Guid, IFunction>();

                program.Id = Id;
                program.Metadata.Clear();

                foreach (var metadataItem in Metadata)
                {
                    program.Metadata[metadataItem.Key] = metadataItem.Value;
                }

                foreach (var functionItem in Functions)
                {
                    var createdFunction = CreateFunction(functionItem, program, builder, loadedPlugins);
                    if (createdFunction != null)
                    {
                        functionIdMap.Add(createdFunction.Id, createdFunction);
                    }
                    else
                    {
                        return false;
                    }
                }

                foreach (var connection in Connections)
                {
                    if (!LinkConnection(connection, functionIdMap))
                    {
                        return false;
                    }
                }

                foreach (var propertyValue in PropertyValues)
                {
                    if (!LinkPropertyValue(propertyValue, functionIdMap))
                    {
                        return false;
                    }
                }

                return true;
            }

            return false;
        }

        private bool LinkPropertyValue(JsonPropertyValue propertyValue, Dictionary<Guid, IFunction> functionIdMap)
        {
            if (functionIdMap.ContainsKey(propertyValue.Function))
            {
                var foundFunction = functionIdMap[propertyValue.Function];
                if (foundFunction != null)
                {
                    var foundProperty = foundFunction.Properties.FirstOrDefault(property => property.Name == propertyValue.Name);
                    if (foundProperty != null)
                    {
                        foundProperty.RawValue = propertyValue.Value;

                        return true;
                    }
                }
            }

            return false;
        }

        private bool LinkConnection(JsonConnection connection, Dictionary<Guid, IFunction> functionIdMap)
        {
            if (functionIdMap.ContainsKey(connection.StartFunctionId) && functionIdMap.ContainsKey(connection.EndFunctionId))
            {
                var startFunction = functionIdMap[connection.StartFunctionId];
                var endFunction = functionIdMap[connection.EndFunctionId];
                var outputName = connection.OutputName;
                var inputName = connection.InputName;
                var connectionType = connection.TypeOfConnection;

                if (!string.IsNullOrEmpty(outputName) && !string.IsNullOrEmpty(inputName) && connectionType != JsonConnection.ConnectionType.Unknown)
                {
                    switch (connectionType)
                    {
                        case JsonConnection.ConnectionType.InputOutput:
                            return LinkInputOutput(connection, startFunction, endFunction, outputName, inputName);
                        case JsonConnection.ConnectionType.PropertyResult:
                            return LinkPropertyResult(connection, startFunction, endFunction, outputName, inputName);
                    }
                }
            }

            return false;
        }

        private bool LinkInputOutput(JsonConnection connection, IFunction startFunction, IFunction endFunction, string outputName, string inputName)
        {
            if (startFunction != null && endFunction != null)
            {
                var output = startFunction.Outputs.FirstOrDefault(output => output.Name == outputName);
                var input = endFunction.Inputs.FirstOrDefault(input => input.Name == inputName);

                if (output != null && input != null && output.Connect(input))
                {
                    SyncMetadata(connection, input, output);

                    return true;
                }
            }

            return false;
        }

        private bool LinkPropertyResult(JsonConnection connection, IFunction startFunction, IFunction endFunction, string outputName, string inputName)
        {
            if (startFunction != null && endFunction != null)
            {
                var result = startFunction.Results.FirstOrDefault(output => output.Name == outputName);
                var property = endFunction.Properties.FirstOrDefault(input => input.Name == inputName);

                if (result != null && property != null && result.Connect(property))
                {
                    SyncMetadata(connection, property, result);

                    return true;
                }
            }

            return false;
        }

        private void SyncMetadata(JsonConnection connection, IInputable inputable, IOutputable outputable)
        {
            if (outputable is IStored storedOutputable)
            {
                foreach (var metadata in connection.OutputMetadata)
                {
                    storedOutputable.Metadata[metadata.Key] = metadata.Value;
                }
            }

            if (inputable is IStored storedInputable)
            {
                foreach (var metadata in connection.InputMetadata)
                {
                    storedInputable.Metadata[metadata.Key] = metadata.Value;
                }
            }
        }

        private IFunction CreateFunction(JsonFunction functionItem, IProgram program, IFunctionBuilder builder, ILoadedPlugins loadedPlugins)
        {
            var availableFunctionTypes = loadedPlugins.Plugins.SelectMany(pluginContainer => pluginContainer.FunctionTypes);

            var foundFunctionType = availableFunctionTypes.FirstOrDefault(functionType => functionType.AssemblyQualifiedName == functionItem.FunctionClass);
            if (foundFunctionType != null)
            {
                var functionInstance = builder.BuildInstance(foundFunctionType);
                if (functionInstance != null && functionItem.Set(functionInstance) && program.AddFunction(functionInstance))
                {
                    return functionInstance;
                }
            }

            return null;
        }
    }
}
