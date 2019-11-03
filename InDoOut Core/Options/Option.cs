using InDoOut_Core.Basic;

namespace InDoOut_Core.Options
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

        /// <summary>
        /// Checks for eqaulity with another object. If the other object is of type <see cref="IOption"/>
        /// it will compare the name.
        /// </summary>
        /// <param name="obj">The object to compare.</param>
        /// <returns>Whether the objects are equal.</returns>
        public override bool Equals(object obj)
        {
            return obj is IOption otherOption ? otherOption.Name == Name : base.Equals(obj);
        }

        /// <summary>
        /// Serves as the default hash function.
        /// </summary>
        /// <returns>The object hash.</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Gets the string representation of this option.
        /// </summary>
        /// <returns>The string representation of this option.</returns>
        public override string ToString()
        {
            return $"[OPTION [Name: {Name}] [Description: {Description}] [Type: {GetType()}] [ValueType: {typeof(ValueType)}] {base.ToString()}]";
        }
    }
}
