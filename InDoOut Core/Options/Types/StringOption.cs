namespace InDoOut_Core.Options.Types
{
    /// <summary>
    /// A string based option where a user can enter any string they want.
    /// </summary>
    public class StringOption : Option<string>
    {
        /// <summary>
        /// Creates a basic string option that allows a user to enter any string.
        /// </summary>
        /// <param name="name">The name of the option.</param>
        /// <param name="description">The description of the option.</param>
        /// <param name="defaultValue">The default value of the option.</param>
        public StringOption(string name, string description = "", string defaultValue = default) : base(name, description, defaultValue)
        {
        }
    }
}
