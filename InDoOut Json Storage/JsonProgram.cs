﻿using InDoOut_Core.Basic;
using InDoOut_Core.Entities.Core;
using InDoOut_Core.Entities.Functions;
using InDoOut_Core.Entities.Programs;
using InDoOut_Core.Functions;
using InDoOut_Core.Logging;
using InDoOut_Core.Reporting;
using InDoOut_Function_Plugins.Containers;
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
        /// A list of failure ids that could occur.
        /// </summary>
        public enum FailureIds
        {
            /// <summary>
            /// Resources required are not available
            /// </summary>
            InvalidResources,

            /// <summary>
            /// A function could not be created.
            /// </summary>
            FailedToCreateFunction,

            /// <summary>
            /// A function was not found.
            /// </summary>
            FunctionNotFound,

            /// <summary>
            /// A function was found, but invalid.
            /// </summary>
            InvalidFunction,

            /// <summary>
            /// A property was not valid.
            /// </summary>
            InvalidProperty,

            /// <summary>
            /// A connection was not valid.
            /// </summary>
            InvalidConnection,

            /// <summary>
            /// The connection type given was unknown.
            /// </summary>
            UnknownConnectionType,

            /// <summary>
            /// A connection could not be made.
            /// </summary>
            ConnectionFailed
        }

        private enum MatchType
        {
            Exact,
            VersionAbove,
            VersionBelow
        }

        /// <summary>
        /// The program name.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; } = "Unnamed";

        /// <summary>
        /// The version of the program saved.
        /// </summary>
        [JsonProperty("version")]
        public Version Version { get; set; } = new Version(0, 1);

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
                    Metadata = program.Metadata,
                    Name = program.Name
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
        public List<IFailureReport> Set(IProgram program, IFunctionBuilder builder, ILoadedPlugins loadedPlugins)
        {
            var failures = new List<IFailureReport>();

            if (program != null && builder != null && loadedPlugins != null)
            {
                var functionIdMap = new Dictionary<Guid, IFunction>();

                ClearProgram(program);

                program.Id = Id;
                program.SetName(Name);

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
                        failures.Add(new FailureReport((int)FailureIds.FailedToCreateFunction, $"The function \"{functionItem?.FunctionName ?? "unknown function"}\" couldn't be created from the plugin {functionItem?.FunctionLibrary ?? "unknown plugin"}."));
                    }
                }

                foreach (var connection in Connections)
                {
                    failures.AddRange(LinkConnection(connection, functionIdMap));
                }

                foreach (var propertyValue in PropertyValues)
                {
                    failures.AddRange(LinkPropertyValue(propertyValue, functionIdMap));
                }
            }
            else
            {
                Log.Instance.Error($"(Program: {program?.Name ?? "null program"}, builder: {builder?.GetHashCode().ToString() ?? "unknown builder"}, loaded plugins: {loadedPlugins?.Plugins?.Count.ToString() ?? "unknown plugins"}");

                failures.Add(new FailureReport((int)FailureIds.InvalidResources, $"The program could not be built due to missing resources.", true));
            }

            return failures;
        }

        private void ClearProgram(IProgram program)
        {
            if (program != null)
            {
                program.Stop();
                program.SetName(null);
                program.Metadata.Clear();

                foreach (var function in program.Functions)
                {
                    _ = program.RemoveFunction(function);
                }
            }
        }

        private List<IFailureReport> LinkPropertyValue(JsonPropertyValue propertyValue, Dictionary<Guid, IFunction> functionIdMap)
        {
            var failures = new List<IFailureReport>();

            if (functionIdMap.ContainsKey(propertyValue.Function))
            {
                var foundFunction = functionIdMap[propertyValue.Function];
                if (foundFunction != null)
                {
                    var foundProperty = foundFunction.Properties.FirstOrDefault(property => property.Name == propertyValue.Name);
                    if (foundProperty != null)
                    {
                        foundProperty.RawValue = propertyValue.Value;
                    }
                    else
                    {
                        failures.Add(new FailureReport((int)FailureIds.InvalidProperty, $"Failed to link the property \"{propertyValue?.Name ?? "null property"}\" to a function, as the function \"{foundFunction?.SafeName ?? "unknown ID"}\" doesn't contain a property with that name."));
                    }
                }
                else
                {
                    failures.Add(new FailureReport((int)FailureIds.InvalidFunction, $"Failed to link the property \"{propertyValue?.Name ?? "null property"}\" to a function ID of \"{propertyValue?.Function.ToString() ?? "unknown ID"}\" doesn't exist."));
                }
            }
            else
            {
                failures.Add(new FailureReport((int)FailureIds.FunctionNotFound, $"Failed to link the property \"{propertyValue?.Name ?? "null property"}\" to a function ID of \"{propertyValue?.Function.ToString() ?? "unknown ID"}\" as it could not be found."));
            }

            return failures;
        }

        private List<IFailureReport> LinkConnection(JsonConnection connection, Dictionary<Guid, IFunction> functionIdMap)
        {
            var failures = new List<IFailureReport>();

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
                            failures.AddRange(LinkInputOutput(connection, startFunction, endFunction, outputName, inputName));
                            break;
                        case JsonConnection.ConnectionType.PropertyResult:
                            failures.AddRange(LinkPropertyResult(connection, startFunction, endFunction, outputName, inputName));
                            break;
                        default:
                            failures.Add(new FailureReport((int)FailureIds.UnknownConnectionType, $"Couldn't create a connection, as the connection type \"{connectionType}\" is unknown to the linker."));
                            break;
                    }
                }
                else
                {
                    failures.Add(new FailureReport((int)FailureIds.InvalidConnection, $"Couldn't create a connection as either the output name ({outputName ?? "unknown name"}), input name ({inputName ?? "unknown name"}) or the connection type \"{connectionType}\" is unknown."));
                }
            }
            else
            {
                failures.Add(new FailureReport((int)FailureIds.FunctionNotFound, $"Failed to link a connection between \"{connection?.InputName ?? "unknown connection"}\" and \"{connection?.OutputName ?? "unknown connection"}\" to their respective functions as either function ID \"{connection?.StartFunctionId.ToString() ?? "unknown connection"}\" or \"{connection?.EndFunctionId.ToString() ?? "unknown connection"}\" doesn't exist."));
            }

            return failures;
        }

        private List<IFailureReport> LinkInputOutput(JsonConnection connection, IFunction startFunction, IFunction endFunction, string outputName, string inputName)
        {
            var failures = new List<IFailureReport>();

            if (startFunction != null && endFunction != null)
            {
                var output = startFunction.Outputs.FirstOrDefault(output => output.Name == outputName);
                var input = endFunction.Inputs.FirstOrDefault(input => input.Name == inputName);

                if (output != null && input != null && output.Connect(input))
                {
                    SyncMetadata(connection, input, output);
                }
                else
                {
                    failures.Add(new FailureReport((int)FailureIds.ConnectionFailed, $"A connection could not be made between input \"{input?.Name ?? $"unknown\" (should be \"{inputName}\")"} on function \"{endFunction?.SafeName ?? "unknown function"}\" ({endFunction?.Id.ToString() ?? "unknown function"}) and output \"{output?.Name ?? $"unknown\" (should be \"{outputName}\")"} on function \"{startFunction?.SafeName ?? "unknown function"}\" ({startFunction?.Id.ToString() ?? "unknown function"})."));
                }
            }
            else
            {
                failures.Add(new FailureReport((int)FailureIds.FunctionNotFound, $"An output could not be linked to an input because one or both of the functions given were null (start function: \"{startFunction?.SafeName ?? "null"}\", end function: \"{endFunction?.SafeName ?? "null"}\")."));
            }

            return failures;
        }

        private List<IFailureReport> LinkPropertyResult(JsonConnection connection, IFunction startFunction, IFunction endFunction, string outputName, string inputName)
        {
            var failures = new List<IFailureReport>();

            if (startFunction != null && endFunction != null)
            {
                var result = startFunction.Results.FirstOrDefault(output => output.Name == outputName);
                var property = endFunction.Properties.FirstOrDefault(input => input.Name == inputName);

                if (result != null && property != null && result.Connect(property))
                {
                    SyncMetadata(connection, property, result);
                }
                else
                {
                    failures.Add(new FailureReport((int)FailureIds.ConnectionFailed, $"A connection could not be made between result \"{result?.Name ?? "null result"}\" on function \"{startFunction?.SafeName ?? "unknown function"}\" ({startFunction?.Id.ToString() ?? "unknown function"}) and property \"{property?.Name ?? "null property"}\" on function \"{endFunction?.SafeName ?? "unknown function"}\" ({endFunction?.Id.ToString() ?? "unknown function"})."));
                }
            }
            else
            {
                failures.Add(new FailureReport((int)FailureIds.FunctionNotFound, $"A result could not be linked to an property because one or both of the functions given were null (start function: \"{startFunction?.SafeName ?? "null"}\", end function: \"{endFunction?.SafeName ?? "null"}\")."));
            }

            return failures;
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
            var availableFunctionTypes = loadedPlugins.Plugins.Where(pluginContainer => pluginContainer is IFunctionPluginContainer).Cast<IFunctionPluginContainer>().SelectMany(pluginContainer => pluginContainer.FunctionTypes);
            var findOrder = new List<MatchType> { MatchType.Exact, MatchType.VersionAbove, MatchType.VersionBelow };

            Type foundFunctionType = null;

            foreach (var matchType in findOrder)
            {
                var matchedType = FindFunctionType(availableFunctionTypes, functionItem, matchType);
                if (matchedType != null)
                {
                    foundFunctionType = matchedType;
                    break;
                }
            }

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

        private Type FindFunctionType(IEnumerable<Type> typeList, JsonFunction function, MatchType matchType)
        {
            var name = function?.FunctionName;
            var library = function?.FunctionLibrary;
            var version = function?.FunctionVersion;

            if (name != null && library != null && version != null)
            {
                foreach (var type in typeList)
                {
                    var typeName = type?.FullName;
                    var typeLibrary = type?.Assembly?.GetName()?.Name;
                    var typeVersion = type?.Assembly?.GetName()?.Version;

                    if (name == typeName && library == typeLibrary)
                    {
                        var versionMatches = (matchType == MatchType.Exact && version == typeVersion) || (matchType == MatchType.VersionAbove && typeVersion > version) || (matchType == MatchType.VersionBelow && typeVersion < version);
                        if (versionMatches)
                        {
                            return type;
                        }
                    }
                }
            }

            return null;
        }
    }
}
