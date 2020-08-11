using InDoOut_Core.Entities.Functions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace InDoOut_Json_Storage
{
    /// <summary>
    /// Represents a JSON shell for a <see cref="IFunction"/>
    /// </summary>
    public class JsonFunction
    {
        private string _functionClass = null;

        /// <summary>
        /// Function Id
        /// </summary>
        [JsonProperty("id")]
        public Guid Id { get; set; } = Guid.Empty;

        /// <summary>
        /// Function associated class
        /// </summary>
        [JsonProperty("class")]
        public string FunctionClass { get => _functionClass; set => FunctionClassUpdated(value); }

        /// <summary>
        /// The name of the function, taken from the <see cref="FunctionClass"/>.
        /// </summary>
        [JsonIgnore]
        public string FunctionName { get; private set; } = null;

        /// <summary>
        /// The name of the library this function belongs to, taken from the <see cref="FunctionClass"/>.
        /// </summary>
        [JsonIgnore]
        public string FunctionLibrary { get; private set; } = null;

        /// <summary>
        /// The version of the function, taken from the <see cref="FunctionClass"/>.
        /// </summary>
        [JsonIgnore]
        public Version FunctionVersion { get; private set; } = null;

        /// <summary>
        /// Function metadata
        /// </summary>
        [JsonProperty("metadata")]
        public Dictionary<string, string> Metadata { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// Creates a <see cref="JsonFunction"/> shell for a given <paramref name="function"/>.
        /// </summary>
        /// <param name="function">The function to construct a JSON shell for.</param>
        /// <returns>A created JSON shell, or null if failed.</returns>
        public static JsonFunction CreateFromFunction(IFunction function)
        {
            if (function != null)
            {
                var jsonFunction = new JsonFunction()
                {
                    Id = function.Id,
                    FunctionClass = function.GetType().AssemblyQualifiedName,
                    Metadata = function.Metadata
                };

                return jsonFunction;
            }

            return null;
        }

        /// <summary>
        /// Sets the data in a given <paramref name="function"/> from the stored data.
        /// </summary>
        /// <param name="function">The function to set.</param>
        /// <returns>Whether the data was set successfully.</returns>
        public bool Set(IFunction function)
        {
            if (function != null)
            {
                function.Id = Id;
                function.Metadata.Clear();

                foreach (var metadataItem in Metadata)
                {
                    function.Metadata[metadataItem.Key] = metadataItem.Value;
                }

                return true;
            }

            return false;
        }

        private void FunctionClassUpdated(string functionClass)
        {
            _functionClass = functionClass;

            ExtractDetailsFromFunctionClass();
        }

        private void ExtractDetailsFromFunctionClass()
        {
            if (!string.IsNullOrEmpty(FunctionClass))
            {
                var detailExtractingRegex = new Regex(@"(?<function>.*?), *(?<library>.*?),.*(?<versionFull>Version=(?<version>\d+\.\d+\.\d+\.\d+)),");
                var detailsMatch = detailExtractingRegex.Match(FunctionClass);

                if (detailsMatch.Success)
                {
                    var groups = detailsMatch.Groups;
                    if (groups.ContainsKey("function") && groups.ContainsKey("library") && groups.ContainsKey("version"))
                    {
                        var version = groups["version"]?.Value;

                        FunctionVersion = null;

                        if (Version.TryParse(version, out var parsedVersion))
                        {
                            FunctionVersion = parsedVersion;
                        }

                        FunctionName = groups["function"]?.Value;
                        FunctionLibrary = groups["library"]?.Value;
                    }
                }
            }
        }
    }
}
