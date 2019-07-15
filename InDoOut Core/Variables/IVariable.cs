using InDoOut_Core.Entities.Core;
using System;

namespace InDoOut_Core.Variables
{
    /// <summary>
    /// Represents an individual variable. Variables have names and values which store state
    /// over multiple functions.
    /// </summary>
    public interface IVariable : INamed
    {
        /// <summary>
        /// Returns whether the variable has a valid name and value
        /// </summary>
        bool Valid { get; }

        /// <summary>
        /// The value associated with this variable in string format.
        /// </summary>
        string Value { get; set; }

        /// <summary>
        /// Gets the value of the variable, or <paramref name="defaultValue"/> if null.
        /// </summary>
        /// <param name="defaultValue">The value to return on null.</param>
        /// <returns>The variable value, or default if null.</returns>
        string ValueOrDefault(string defaultValue = "");

        /// <summary>
        /// Converts the <see cref="Value"/> to the specified type <typeparamref name="T"/>. Failing that it will
        /// fall back to <paramref name="defaultValue"/>.
        /// </summary>
        /// <typeparam name="T">The type to convert to.</typeparam>
        /// <param name="defaultValue">The default value to output in case of a conversion failure.</param>
        /// <returns>The <see cref="Value"/> as the requested type <typeparamref name="T"/>.</returns>
        T ValueAs<T>(T defaultValue = default);
    }
}
