namespace InDoOut_Core.Basic
{
    /// <summary>
    /// Represents a basic value as well as conversion utilities.
    /// </summary>
    public interface IValue
    {
        /// <summary>
        /// Sets the value from the given type <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T">The type to set the value from.</typeparam>
        /// <param name="value">The type value to set the value to.</param>
        /// <returns>Whether the value was converted and set.</returns>
        bool ValueFrom<T>(T value);

        /// <summary>
        /// Returns whether the value currently stored is valid.
        /// </summary>
        bool ValidValue { get; }

        /// <summary>
        /// The value associated with this variable in string format.
        /// </summary>
        string RawValue { get; set; }

        /// <summary>
        /// Gets the value, or <paramref name="defaultValue"/> if null.
        /// </summary>
        /// <param name="defaultValue">The value to return on null.</param>
        /// <returns>The value, or default if null.</returns>
        string ValueOrDefault(string defaultValue = "");

        /// <summary>
        /// Converts the <see cref="RawValue"/> to the specified type <typeparamref name="T"/>. Failing that it will
        /// fall back to <paramref name="defaultValue"/>.
        /// </summary>
        /// <typeparam name="T">The type to convert to.</typeparam>
        /// <param name="defaultValue">The default value to output in case of a conversion failure.</param>
        /// <returns>The <see cref="RawValue"/> as the requested type <typeparamref name="T"/>.</returns>
        T ValueAs<T>(T defaultValue = default);
    }
}
