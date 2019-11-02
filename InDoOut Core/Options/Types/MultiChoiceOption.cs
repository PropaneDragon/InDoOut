using System.Collections.Generic;
using System.Linq;

namespace InDoOut_Core.Options.Types
{
    /// <summary>
    /// A multi-choice option where a user can select a value from a list of possible values.
    /// </summary>
    public class MultiChoiceOption : Option<string>
    {
        /// <summary>
        /// The available items to select from in the option.
        /// </summary>
        public List<string> Items { get; private set; } = new List<string>();

        /// <summary>
        /// Creates a basic multi choice option with a name, description, default value and list of items.
        /// </summary>
        /// <param name="name">The name of the option.</param>
        /// <param name="description">A more detailed description of the option.</param>
        /// <param name="defaultValue">The default value of the option.</param>
        /// <param name="items">The available items to be selected from.</param>
        public MultiChoiceOption(string name, string description, string defaultValue, params string[] items) : base(name, description, defaultValue)
        {
            Items = items.ToList();
        }
    }
}
