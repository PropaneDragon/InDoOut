namespace InDoOut_Core.Options.Types
{
    /// <summary>
    /// An option that is hidden from any sort of interface.
    /// </summary>
    public class HiddenStringOption : Option<string>
    {
        /// <summary>
        /// Creates a basic hidden option with name, description and default value.
        /// </summary>
        /// <param name="name">The name of the option.</param>
        /// <param name="description">A more detailed description of what the option does.</param>
        /// <param name="defaultValue">The default value of the option before the user interacts with it.</param>
        public HiddenStringOption(string name, string description = "", string defaultValue = "") : base(name, description, defaultValue)
        {
        }
    }
}
