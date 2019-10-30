namespace InDoOut_Plugins.Options.Types
{
    /// <summary>
    /// An option that can be checked or unchecked.
    /// </summary>
    public class CheckableOption : Option<bool>
    {
        /// <summary>
        /// Creates a basic checkable option with name, description and default value.
        /// </summary>
        /// <param name="name">The name of the option.</param>
        /// <param name="description">A more detailed description of what the option does.</param>
        /// <param name="defaultValue">The default value of the option before the user interacts with it.</param>
        public CheckableOption(string name, string description = "", bool defaultValue = false) : base(name, description, defaultValue)
        {
        }
    }
}
