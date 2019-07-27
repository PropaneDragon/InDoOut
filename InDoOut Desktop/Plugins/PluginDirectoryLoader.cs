using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using InDoOut_Desktop.Location;
using InDoOut_Plugins.Containers;
using InDoOut_Plugins.Loaders;

namespace InDoOut_Desktop.Plugins
{
    internal class PluginDirectoryLoader : IPluginDirectoryLoader
    {
        public string PluginExtension { get; set; } = ".dll";
        public IPluginLoader PluginLoader { get; set; } = null;
        public IStandardLocations StandardLocations { get; set; } = null;

        private PluginDirectoryLoader()
        {
        }

        public PluginDirectoryLoader(IPluginLoader pluginLoader, IStandardLocations standardLocations) : this()
        {
            PluginLoader = pluginLoader;
            StandardLocations = standardLocations;
        }

        public async Task<List<IPluginContainer>> LoadPlugins()
        {
            var plugins = new List<IPluginContainer>();
            var standardPluginLocation = StandardLocations?.GetPathTo(Location.Location.PluginsDirectory);

            if (!string.IsNullOrEmpty(standardPluginLocation))
            {
                plugins = await LoadPlugins(standardPluginLocation);
            }

            return plugins;
        }

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
                        FindLibraries(directoryPath, extension);
                    }
                }
            }

            return libraryPaths;
        }
    }
}
