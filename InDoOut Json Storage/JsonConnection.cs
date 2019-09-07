using InDoOut_Core.Entities.Core;
using InDoOut_Core.Entities.Functions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace InDoOut_Json_Storage
{
    /// <summary>
    /// Represents a raw JSON connection between two functions.
    /// </summary>
    public class JsonConnection
    {
        /// <summary>
        /// The type of connection this represents.
        /// </summary>
        public enum ConnectionType
        {
            /// <summary>
            /// An invalid, unknown connection.
            /// </summary>
            Unknown = 0,
            /// <summary>
            /// A connection that consists of <see cref="IInput"/> and <see cref="IOutput"/>.
            /// </summary>
            InputOutput = 1,
            /// <summary>
            /// A connection that consists of <see cref="IProperty"/> and <see cref="IResult"/>.
            /// </summary>
            PropertyResult = 2
        }

        /// <summary>
        /// The type of connection this represents.
        /// </summary>
        [JsonProperty("type")]
        public ConnectionType TypeOfConnection { get; set; } = ConnectionType.Unknown;

        /// <summary>
        /// The name of the output that the connection originates from.
        /// </summary>
        [JsonProperty("output")]
        public string OutputName { get; set; } = null;

        /// <summary>
        /// The name of the input that the connection connects to.
        /// </summary>
        [JsonProperty("input")]
        public string InputName { get; set; } = null;

        /// <summary>
        /// The ID of the function that the connection is connected from.
        /// </summary>
        [JsonProperty("startId")]
        public Guid StartFunctionId { get; set; } = Guid.Empty;

        /// <summary>
        /// The ID of the function that the connection is connected to.
        /// </summary>
        [JsonProperty("endId")]
        public Guid EndFunctionId { get; set; } = Guid.Empty;

        /// <summary>
        /// The metadata applied to the input.
        /// </summary>
        [JsonProperty("inputMetadata")]
        public Dictionary<string, string> InputMetadata { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// The  metadata applied to the output.
        /// </summary>
        [JsonProperty("outputMetadata")]
        public Dictionary<string, string> OutputMetadata { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// Creates a <see cref="JsonFunction"/> shell for a given <paramref name="function"/>.
        /// </summary>
        /// <param name="function">The function to construct a JSON shell for.</param>
        /// <returns>A created JSON shell, or null if failed.</returns>
        public static List<JsonConnection> CreateFromFunction(IFunction function)
        {
            if (function != null)
            {
                var allJsonConnections = new List<JsonConnection>();

                foreach (var output in function.Outputs)
                {
                    var jsonConnections = CreateFromFunction(function, output);
                    if (jsonConnections != null)
                    {
                        allJsonConnections.AddRange(jsonConnections);
                    }
                }

                foreach (var result in function.Results)
                {
                    var jsonConnections = CreateFromFunction(function, result);
                    if (jsonConnections != null)
                    {
                        allJsonConnections.AddRange(jsonConnections);
                    }
                }

                return allJsonConnections;
            }

            return null;
        }

        private static List<JsonConnection> CreateFromFunction(IFunction function, IOutputable outputable)
        {
            if (function != null && outputable != null)
            {
                var jsonConnections = new List<JsonConnection>();
                var rawInputs = outputable.RawConnections;

                foreach (var rawInput in rawInputs)
                {
                    JsonConnection jsonConnection = null;

                    if (rawInput is IInput input && outputable is IOutput output)
                    {
                        jsonConnection = CreateFromFunction(function, output, input.Parent, input, ConnectionType.InputOutput);                        
                    }
                    else if (rawInput is IProperty property && outputable is IResult result)
                    {
                        jsonConnection = CreateFromFunction(function, result, property.Parent, property, ConnectionType.PropertyResult);
                    }

                    if (jsonConnection != null)
                    {
                        jsonConnections.Add(jsonConnection);
                    }
                }

                return jsonConnections;
            }

            return null;
        }

        private static JsonConnection CreateFromFunction(IFunction start, IOutputable outputable, IFunction end, IInputable inputable, ConnectionType connectionType)
        {
            if (start != null && outputable != null && end != null && inputable != null)
            {
                if (outputable is INamedEntity namedOutput && inputable is INamedEntity namedInput)
                {
                    return new JsonConnection()
                    {
                        TypeOfConnection = connectionType,
                        StartFunctionId = start.Id,
                        EndFunctionId = end.Id,
                        InputName = namedInput.Name,
                        OutputName = namedOutput.Name,
                        InputMetadata = namedInput.Metadata,
                        OutputMetadata = namedOutput.Metadata
                    };
                }
            }

            return null;
        } 
    }
}
