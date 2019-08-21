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
                foreach (var connection in function.Connections)
                {
                    var jsonConnections = CreateFromFunction(function, connection);
                    if (jsonConnections != null)
                    {
                        allJsonConnections.AddRange(jsonConnections);
                    }
                }

                return allJsonConnections;
            }

            return null;
        }

        private static List<JsonConnection> CreateFromFunction(IFunction function, IOutput connection)
        {
            if (function != null && connection != null)
            {
                var jsonConnections = new List<JsonConnection>();
                var inputs = connection.Connections;

                foreach (var input in inputs)
                {
                    var endFunction = input.Parent;
                    var jsonConnection = CreateFromFunction(function, connection, endFunction, input);
                    if (jsonConnection != null)
                    {
                        jsonConnections.Add(jsonConnection);
                    }
                }

                return jsonConnections;
            }

            return null;
        }

        private static JsonConnection CreateFromFunction(IFunction start, IOutput output, IFunction end, IInput input)
        {
            if (start != null && output != null && end != null && input != null)
            {
                return new JsonConnection()
                {
                    StartFunctionId = start.Id,
                    EndFunctionId = end.Id,
                    InputName = input.Name,
                    OutputName = output.Name,
                    InputMetadata = input.Metadata,
                    OutputMetadata = output.Metadata
                };
            }

            return null;
        }
    }
}
