using InDoOut_Core.Threading.Safety;
using System;

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
        public bool Valid => !string.IsNullOrEmpty(Name) && !string.IsNullOrEmpty(Value);

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
        public T ValueAs<T>(T defaultValue = default) where T : IConvertible
        {
            var convertedValue = TryGet.ValueOrDefault(() => (T)Convert.ChangeType(Value, typeof(T)));

            return convertedValue != null ? convertedValue : defaultValue;
        }

        /// <summary>
        /// Checks equality on the variable. Variables are case insensitive.
        /// </summary>
        /// <param name="obj">The object to compare againt.</param>
        /// <returns>Whether the variable is equal to the given object <paramref name="obj"/>.</returns>
        public override bool Equals(object obj)
        {
            return obj is Variable variable &&
                   Name?.ToLower() == variable.Name?.ToLower() &&
                   Value == variable.Value;
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
