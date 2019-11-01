using InDoOut_Core.Basic;
using InDoOut_Core.Threading.Safety;

namespace InDoOut_Plugins.Options
{
    /// <summary>
    /// An option that can be modified by the user.
    /// </summary>
    public abstract class Option<ValueType> : Value, IOption<ValueType>
    {
        /// <summary>
        /// Whether this option is visible to the user or not.
        /// </summary>
        public bool Visible { get; protected set; } = true;

        /// <summary>
        /// A description of what the option changes in more detail.
        /// </summary>
        public string Description { get; private set; } = null;

        /// <summary>
        /// The name of the option.
        /// </summary>
        public string Name { get; private set; } = null;

        /// <summary>
        /// The current value of the option.
        /// </summary>
        public ValueType Value { get => ValueAs<ValueType>(); set => ValueFrom(value); }

        private Option()
        {
        }

        /// <summary>
        /// Creates a basic option with a name and optional description.
        /// </summary>
        /// <param name="name">The name of the option.</param>
        /// <param name="description">A more detailed description of what the option is for.</param>
        /// <param name="defaultValue">The default value of the option before a user changes it.</param>
        public Option(string name, string description = "", ValueType defaultValue = default) : this()
        {
            Name = name;
            Description = description;
            Value = defaultValue;
        }
    }
}
