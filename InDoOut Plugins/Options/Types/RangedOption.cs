namespace InDoOut_Plugins.Options.Types
{
    /// <summary>
    /// An abstract ranged option. These store a minimum and maximum possible value for the option and only
    /// allow the user to select a value between these values.
    /// </summary>
    /// <typeparam name="RangedType">The type to store.</typeparam>
    public abstract class RangedOption<RangedType> : Option<RangedType>
    {
        /// <summary>
        /// The inclusive minimum value allowed for the option.
        /// </summary>
        public RangedType MinimumValue { get; private set; } = default;

        /// <summary>
        /// The inclusive maximum value allowed for the option.
        /// </summary>
        public RangedType MaximumValue { get; private set; } = default;

        /// <summary>
        /// Creates a basic ranged option with a name, minimum value, maximum values, description and default value.
        /// </summary>
        /// <param name="name">The name of the option.</param>
        /// <param name="minimumValue">The inclusive minimum value allowed for the option.</param>
        /// <param name="maximumValue">The inclusive maximum value allowed for the option.</param>
        /// <param name="description">A more detailed description of what the option is for.</param>
        /// <param name="defaultValue">The default value given before the user sets it.</param>
        public RangedOption(string name, RangedType minimumValue, RangedType maximumValue, string description = "", RangedType defaultValue = default) : base(name, description, defaultValue)
        {
            MinimumValue = minimumValue;
            MaximumValue = maximumValue;
        }

        /// <summary>
        /// Checks whether the option value is within the minimum and maximum values.
        /// </summary>
        /// <returns>Whether the option value is within the minimum and maximum values.</returns>
        public abstract bool ValueWithinBounds();
    }
}
