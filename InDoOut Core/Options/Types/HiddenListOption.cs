using System.Collections.Generic;
using System.Linq;

namespace InDoOut_Core.Options.Types
{
    /// <summary>
    /// An option that is hidden from any sort of interface. This converts between lists and strings
    /// internally so it can store and retrieve strings from the internal option storage that it
    /// stores as a string.
    /// </summary>
    public class HiddenListOption : Option<string>
    {
        private const string DEFAULT_LIST_OPTION_DELIMITER = ";";

        private readonly string _delimiter = DEFAULT_LIST_OPTION_DELIMITER;

        /// <summary>
        /// The converted list from the base storage type of a string.
        /// </summary>
        public List<string> ListValue { get => ConvertFromValue(); set => ConvertToValue(value); }

        /// <summary>
        /// Creates a hidden option that can store a list of strings with name, description and default value.
        /// </summary>
        /// <param name="name">The name of the option.</param>
        /// <param name="description">A more detailed description of what the option does.</param>
        /// <param name="defaultValue">The default value of the option before the user interacts with it.</param>
        /// <param name="delimiter">The delimiter to store the data as.</param>
        public HiddenListOption(string name, string description = "", List<string> defaultValue = default, string delimiter = DEFAULT_LIST_OPTION_DELIMITER) : base(name, description)
        {
            _delimiter = delimiter;
            ListValue = defaultValue;
        }

        private void ConvertToValue(List<string> list)
        {
            Value = string.Join(_delimiter, list ?? new List<string>());
        }

        private List<string> ConvertFromValue()
        {
            return Value?.Split(_delimiter, System.StringSplitOptions.RemoveEmptyEntries)?.ToList() ?? new List<string>();
        }
    }
}
