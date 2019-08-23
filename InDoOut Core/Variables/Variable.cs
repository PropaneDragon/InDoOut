using InDoOut_Core.Basic;

namespace InDoOut_Core.Variables
{
    /// <summary>
    /// An individual variable that holds a value and a name.
    /// </summary>
    public class Variable : Value, IVariable
    {
        /// <summary>
        /// The variable name.
        /// </summary>
        public string Name { get; } = null;

        /// <summary>
        /// Returns whether the variable is valid or not.
        /// </summary>
        public bool Valid => Name != null && ValidValue;

        private Variable() { }

        /// <summary>
        /// Creates a new variable with the given name <paramref name="name"/>.
        /// </summary>
        /// <param name="name">The name to associate with the variable.</param>
        /// <param name="value">The value of the variable.</param>
        public Variable(string name, string value = "") : this()
        {
            Name = name;
            RawValue = value;
        }

        /// <summary>
        /// Checks equality on the variable. Variables are case insensitive.
        /// </summary>
        /// <param name="obj">The object to compare againt.</param>
        /// <param name="includeValues">Whether or not to include the value of the variable in the comparison.</param>
        /// <returns>Whether the variable is equal to the given object <paramref name="obj"/>.</returns>
        public bool Equals(object obj, bool includeValues)
        {
            return Equals(obj) &&
                   obj is Variable variable &&
                   (!includeValues || RawValue == variable.RawValue);
        }

        /// <summary>
        /// Checks equality on the variable. Variables are case insensitive.
        /// </summary>
        /// <param name="obj">The object to compare againt.</param>
        /// <returns>Whether the variable is equal to the given object <paramref name="obj"/>.</returns>
        public override bool Equals(object obj)
        {
            return obj is Variable variable &&
                   Name?.ToLower() == variable.Name?.ToLower();
        }

        /// <summary>
        /// <see cref="object.GetHashCode"/>.
        /// </summary>
        /// <returns>The hash code for this variable.</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
