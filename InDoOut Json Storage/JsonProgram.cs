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
        /// Program Id
        /// </summary>
        [JsonProperty("id")]
        public Guid Id { get; set; } = Guid.Empty;

        /// <summary>
        /// Program functions.
        /// </summary>
        [JsonProperty("functions")]
        public List<JsonFunction> Functions { get; set; } = new List<JsonFunction>();

        /// <summary>
        /// Program, function connections
        /// </summary>
        [JsonProperty("connections")]
        public List<JsonConnection> Connections { get; set; } = new List<JsonConnection>();

        /// <summary>
        /// Program metadata
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
                var availableFunctionTypes = loadedPlugins.Plugins.SelectMany(pluginContainer => pluginContainer.FunctionTypes);
                var functionIdMap = new Dictionary<Guid, IFunction>();

                program.Id = Id;
                program.Metadata.Clear();

                foreach (var metadataItem in Metadata)
                {
                    program.Metadata[metadataItem.Key] = metadataItem.Value;
                }

                foreach (var functionItem in Functions)
                {
                    var foundFunctionType = availableFunctionTypes.FirstOrDefault(functionType => functionType.AssemblyQualifiedName == functionItem.FunctionClass);
                    if (foundFunctionType != null)
                    {
                        var functionInstance = builder.BuildInstance(foundFunctionType);
                        if (functionInstance != null && functionItem.Set(functionInstance) && program.AddFunction(functionInstance))
                        {
                            functionIdMap.Add(functionInstance.Id, functionInstance);
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }

                foreach (var connection in Connections)
                {
                    if (functionIdMap.ContainsKey(connection.StartFunctionId) && functionIdMap.ContainsKey(connection.EndFunctionId))
                    {
                        var startFunction = functionIdMap[connection.StartFunctionId];
                        var endFunction = functionIdMap[connection.EndFunctionId];
                        var outputName = connection.OutputName;
                        var inputName = connection.InputName;

                        if (!string.IsNullOrEmpty(outputName) && !string.IsNullOrEmpty(inputName))
                        {
                            var output = startFunction.Outputs.FirstOrDefault(output => output.Name == outputName);
                            var input = endFunction.Inputs.FirstOrDefault(input => input.Name == inputName);
                            
                            if (output == null || input == null || !output.Connect(input))
                            {
                                return false;
                            }
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }

                return true;
            }

            return false;
        }
    }
}
