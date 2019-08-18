using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using InDoOut_Plugins.Containers;
using InDoOut_Executable_Core.Location;

namespace InDoOut_Plugins.Loaders
{
    /// <summary>
    /// A plugin loader that can load from a directory location.
    /// </summary>
    public class PluginDirectoryLoader : IPluginDirectoryLoader
    {
        /// <summary>
        /// The extension to look for plugins.
        /// </summary>
        public string PluginExtension { get; set; } = ".dll";

        /// <summary>
        /// The current loader for loading in plugins from the directory.
        /// </summary>
        public IPluginLoader PluginLoader { get; set; } = null;

        /// <summary>
        /// The current standard locations to load in plugins.
        /// </summary>
        public IStandardLocations StandardLocations { get; set; } = null;

        private PluginDirectoryLoader()
        {
        }

        /// <summary>
        /// Initialises a directory plugin loader given an individual plugin loader and standard locations to load plugins from.
        /// </summary>
        /// <param name="pluginLoader">The loader to handle loading plugins.</param>
        /// <param name="standardLocations">The standard locations to load plugins from.</param>
        public PluginDirectoryLoader(IPluginLoader pluginLoader, IStandardLocations standardLocations) : this()
        {
            PluginLoader = pluginLoader;
            StandardLocations = standardLocations;
        }

        /// <summary>
        /// Loads plugins from the given standard locations directory.
        /// </summary>
        /// <returns>A list of loaded plugins.</returns>
        public async Task<List<IPluginContainer>> LoadPlugins()
        {
            var plugins = new List<IPluginContainer>();
            var standardPluginLocation = StandardLocations?.GetPathTo(Location.PluginsDirectory);

            if (!string.IsNullOrEmpty(standardPluginLocation))
            {
                plugins = await LoadPlugins(standardPluginLocation);
            }

            return plugins;
        }

        /// <summary>
        /// Loads plugins from a given directory.
        /// </summary>
        /// <param name="directory">The directory path to load plugins from.</param>
        /// <returns>A list of loaded plugins.</returns>
        public async Task<List<IPluginContainer>> LoadPlugins(string directory)
        {
            var plugins = new List<IPluginContainer>();

            if (!string.IsNullOrEmpty(directory) && PluginLoader != null)
            {
                var libraryPaths = await Task.Run(() => FindLibraries(directory, PluginExtension));

                foreach (var libraryPath in libraryPaths)
                {
                    var loadedPlugin = await Task.Run(() => PluginLoader.LoadPlugin(libraryPath));
                    if (loadedPlugin != null)
                    {
                        plugins.Add(loadedPlugin);
                    }
                }
            }

            return plugins;
        }

        /// <summary>
        /// Finds all libraries with a given extension in the specified directory.
        /// </summary>
        /// <param name="directory">The directory to search in.</param>
        /// <param name="extension">The extension to search for.</param>
        /// <returns></returns>
        protected List<string> FindLibraries(string directory, string extension)
        {
            var libraryPaths = new List<string>();

            if (!string.IsNullOrEmpty(directory) && !string.IsNullOrEmpty(extension))
            {
                if (Directory.Exists(directory))
                {
                    var directories = Directory.GetDirectories(directory);
                    var files = Directory.GetFiles(directory, $"*{extension}");

                    libraryPaths.AddRange(files.ToList());

                    foreach (var directoryPath in directories)
                    {
                        libraryPaths.AddRange(FindLibraries(directoryPath, extension));
                    }
                }
            }

            return libraryPaths;
        }
    }
}
