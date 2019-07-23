using InDoOut_Core.Threading.Safety;

namespace InDoOut_Desktop_API.Plugins
{
    /// <summary>
    /// Allows the application to reference the class as a plugin, which
    /// enables it to load in the custom functions and other extensions 
    /// automatically.
    /// </summary>
    public abstract class Plugin : IPlugin
    {
        /// <summary>
        /// Whether this plugin is valid and can be loaded.
        /// </summary>
        public bool Valid => !string.IsNullOrEmpty(TryGet.ValueOrDefault(() => Name));

        /// <summary>
        /// The safe version of <see cref="Name"/>. This has exception handling in the case of an invalid name.
        /// </summary>
        public string SafeName => TryGet.ValueOrDefault(() => Name, "Unknown name");

        /// <summary>
        /// The safe version of <see cref="Description"/>. This has exception handling in the case of an invalid description.
        /// </summary>
        public string SafeDescription => TryGet.ValueOrDefault(() => Description, "");

        /// <summary>
        /// The safe version of <see cref="Author"/>. This has exception handling in the case of an invalid author.
        /// </summary>
        public string SafeAuthor => TryGet.ValueOrDefault(() => Author, "Unknown author");

        /// <summary>
        /// The name of the plugin. Visible to the user.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// A description of what this plugin contains or does. Visible to the user.
        /// </summary>
        public abstract string Description { get; }

        /// <summary>
        /// The author(s) of this plugin. Visible to the user.
        /// </summary>
        public abstract string Author { get; }
    }
}
