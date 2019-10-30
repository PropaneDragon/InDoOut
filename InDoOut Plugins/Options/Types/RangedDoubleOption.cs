﻿namespace InDoOut_Plugins.Options.Types
{
    /// <summary>
    /// A ranged option that stores a double.
    /// See <see cref="RangedOption{RangedType}"/> for further info.
    /// </summary>
    public class RangedDoubleOption : RangedOption<double>
    {
        /// <summary>
        /// Creates a basic ranged option that can store a double value, with an inclusive minimum and maximum value.
        /// </summary>
        /// <param name="name">The name of the option.</param>
        /// <param name="minimumValue">The inclusive minimum allowed value for the option.</param>
        /// <param name="maximumValue">The inclusive maximum allowed value for the option.</param>
        /// <param name="description">A more detailed description of what the option is for.</param>
        /// <param name="defaultValue">The default option value before it is set by the user.</param>
        public RangedDoubleOption(string name, double minimumValue, double maximumValue, string description = "", double defaultValue = 0) : base(name, minimumValue, maximumValue, description, defaultValue)
        {
        }

        /// <summary>
        /// Returns whether the option value is within the minimum and maximum values (inclusive).
        /// </summary>
        /// <returns>Whether the option value is within the minimum and maximum values (inclusive).</returns>
        public override bool ValueWithinBounds() => Value >= MinimumValue && Value <= MaximumValue;
    }
}
