using InDoOut_Core.Entities.Functions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace InDoOut_Json_Storage
{
    /// <summary>
    /// Represents a JSON shell for an <see cref="IProperty"/>.
    /// </summary>
    public class JsonPropertyValue
    {
        /// <summary>
        /// The property value.
        /// </summary>
        [JsonProperty("value")]
        public string Value { get; set; } = null;

        /// <summary>
        /// The property name.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; } = null;

        /// <summary>
        /// The associated function ID.
        /// </summary>
        [JsonProperty("function")]
        public Guid Function { get; set; } = Guid.Empty;

        /// <summary>
        /// Creates a list of <see cref="JsonPropertyValue"/>s for a given <paramref name="function"/>.
        /// </summary>
        /// <param name="function">The function to generate <see cref="JsonPropertyValue"/>s for.</param>
        /// <returns>A list of property values within the given function, or null if a null function has been passed.</returns>
        public static List<JsonPropertyValue> CreateFromFunction(IFunction function)
        {
            if (function != null)
            {
                var values = new List<JsonPropertyValue>();

                foreach (var property in function.Properties)
                {
                    if (property.ValidValue)
                    {
                        values.Add(new JsonPropertyValue()
                        {
                            Function = function.Id,
                            Name = property.Name,
                            Value = property.RawValue
                        });
                    }
                }

                return values;
            }

            return null;
        }

        /// <summary>
        /// Sets a property with a name of <see cref="Name"/> inside a given <paramref name="function"/> to the stored <see cref="Value"/>.
        /// </summary>
        /// <param name="function">The function to set.</param>
        /// <returns>Whether the property was set.</returns>
        public bool Set(IFunction function)
        {
            if (function != null)
            {
                var validProperty = function.Properties.FirstOrDefault(property => property.Name == Name);
                if (validProperty != null)
                {
                    validProperty.RawValue = Value;

                    return true;
                }
            }

            return false;
        }
    }
}
