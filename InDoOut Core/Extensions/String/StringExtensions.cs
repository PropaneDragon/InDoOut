using System;

namespace InDoOut_Core.Extensions.String
{
    /// <summary>
    /// Extensions for handling strings
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Pluralises a string using a given value. If <paramref name="value"/>
        /// is not 1 it will append an 's' to the end of the string.
        /// </summary>
        /// <param name="string">The string to pluralise.</param>
        /// <param name="value">The value to use to pluralise the string.</param>
        /// <returns>A pluralised version of the given string.</returns>
        public static string Pluralise(this string @string, int value) => @string + (Math.Abs(value) == 1 ? "" : "s");
    }
}
