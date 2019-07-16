using InDoOut_Core.Threading.Safety;
using System.ComponentModel;

namespace InDoOut_Core.Variables
{
    /// <summary>
    /// An individual variable that holds a value <see cref="Value"/> for a given name <see cref="Name"/>.
    /// </summary>
    public class Variable : IVariable
    {
        /// <summary>
        /// Whether the variable has a valid name and value.
        /// </summary>
        public bool Valid => !string.IsNullOrEmpty(Name) && Value != null;

        /// <summary>
        /// The name of the variable.
        /// </summary>
        public string Name { get; protected set; } = null;

        /// <summary>
        /// The value associated with the name.
        /// </summary>
        public string Value { get; set; } = "";

        private Variable() { }

        /// <summary>
        /// Creates a new variable with the given name <paramref name="name"/>.
        /// </summary>
        /// <param name="name">The name to associate with the variable.</param>
        /// <param name="value">The value of the variable.</param>
        public Variable(string name, string value = "") : this()
        {
            Name = name;
            Value = value;
        }

        /// <summary>
        /// Returns <see cref="Value"/>, or <paramref name="defaultValue"/> if null.
        /// </summary>
        /// <param name="defaultValue">The value to return if the stored value is null.</param>
        /// <returns>The value of the variable, or <paramref name="defaultValue"/> if null.</returns>
        public string ValueOrDefault(string defaultValue = "") => TryGet.ValueOrDefault(() => Value) ?? defaultValue;

        /// <summary>
        /// Converts the <see cref="Value"/> of the variable to the given type <typeparamref name="T"/>. If this conversion
        /// fails, <paramref name="defaultValue"/> is returned instead.
        /// </summary>
        /// <typeparam name="T">The type to convert to.</typeparam>
        /// <param name="defaultValue">The value to return if the conversion fails.</param>
        /// <returns>The converted value or <paramref name="defaultValue"/> if conversion fails.</returns>
        public T ValueAs<T>(T defaultValue = default)
        {
            return TryGet.ValueOrDefault(() => (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFromString(Value), defaultValue);
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
                   (!includeValues || Value == variable.Value);
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
