using InDoOut_Core.Options;
using Newtonsoft.Json;

namespace InDoOut_Json_Storage
{
    /// <summary>
    /// Represents a basic JSON shell for an <see cref="IOption"/>.
    /// </summary>
    [JsonObject("program")]
    public class JsonOption
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
        /// Creates a JSON representation of an <see cref="IOption"/> for storage.
        /// </summary>
        /// <param name="option">The option to store.</param>
        /// <returns>A JSON representation of the given <paramref name="option"/>.</returns>
        public static JsonOption CreateFromOption(IOption option)
        {
            if (option != null && !string.IsNullOrEmpty(option.Name))
            {
                return new JsonOption()
                {
                    Value = option.RawValue,
                    Name = option.Name
                };
            }

            return null;
        }

        /// <summary>
        /// Sets a given <paramref name="option"/> to the value in this <see cref="JsonOption"/>. Fails if
        /// the names are differnent and <paramref name="failIfNameDifferent"/> is true. Set to false to set
        /// the value regardless of the name matching.
        /// </summary>
        /// <param name="option">The option to set the value of.</param>
        /// <param name="failIfNameDifferent">Returns false if this is true and name of the <paramref name="option"/> and <see cref="Name"/> are different.</param>
        /// <returns>Whether or not the <see cref="Value"/> was applied to the <paramref name="option"/>.</returns>
        public bool Set(IOption option, bool failIfNameDifferent = true)
        {
            return option != null && (option.Name == Name || !failIfNameDifferent) ? option.ValueFrom(Value) : false;
        }
    }
}
