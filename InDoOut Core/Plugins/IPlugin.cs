namespace InDoOut_Core.Plugins
{
    /// <summary>
    /// Represents a plugin that is recognised by the application and loaded in automatically.
    /// </summary>
    public interface IPlugin
    {
        /// <summary>
        /// Whether the plugin is valid and can be loaded.
        /// </summary>
        bool Valid { get; }

        /// <summary>
        /// The name of the plugin. Visible to the user.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The safe version of <see cref="Name"/>. This has exception handling in the case of an invalid name.
        /// </summary>
        string SafeName { get; }

        /// <summary>
        /// A description of what this plugin contains or does. Visible to the user.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// The safe version of <see cref="Description"/>. This has exception handling in the case of an invalid description.
        /// </summary>
        string SafeDescription { get; }

        /// <summary>
        /// The author(s) of this plugin. Visible to the user.
        /// </summary>
        string Author { get; }

        /// <summary>
        /// The safe version of <see cref="Author"/>. This has exception handling in the case of an invalid author.
        /// </summary>
        string SafeAuthor { get; }
    }
}
