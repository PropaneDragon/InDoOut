using InDoOut_Core.Reporting;
using InDoOut_Display_Core.Elements;
using InDoOut_Plugins.Loaders;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace InDoOut_Display_Json_Storage
{
    /// <summary>
    /// Represents a basic JSON shell for an <see cref="IDisplayElement"/>.
    /// </summary>
    [JsonObject("program")]
    public class JsonDisplayElement
    {
        /// <summary>
        /// A list of failure ids that could occur.
        /// </summary>
        public enum FailureIds
        {
        }

        /// <summary>
        /// Elemment internal function Id.
        /// </summary>
        [JsonProperty("functionId")]
        public Guid FunctionId { get; set; } = Guid.Empty;

        /// <summary>
        /// Elemment internal entity Id.
        /// </summary>
        [JsonProperty("entityId")]
        public Guid EntityId { get; set; } = Guid.Empty;

        /// <summary>
        /// Element metadata.
        /// </summary>
        [JsonProperty("metadata")]
        public Dictionary<string, string> Metadata { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// Creates a <see cref="JsonDisplayElement"/> shell for a given <paramref name="element"/>.
        /// </summary>
        /// <param name="element">The element to construct a JSON shell for.</param>
        /// <returns>A created JSON shell, or null if failed.</returns>
        public static JsonDisplayElement CreateFromDisplayElement(IDisplayElement element)
        {
            /*if (element != null)
            {
                var jsonElement = new JsonDisplayElement()
                {

                }
            }*/

            return null;
        }

        public List<IFailureReport> Set(IDisplayElement element, ILoadedPlugins loadedPlugins)
        {
            return new List<IFailureReport>();
        }
    }
}
