using InDoOut_Core.Instancing;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace InDoOut_Executable_Core.Location
{
    public class StandardLocations : Singleton<StandardLocations>, IStandardLocations
    {
        private readonly Dictionary<Location, string> _paths = new Dictionary<Location, string>();

        private readonly List<Location> _editablePaths = new List<Location>()
        {
            Location.SaveFile,
            Location.PluginsDirectory,
            Location.SettingsFile
        };

        public static char Separator => Path.DirectorySeparatorChar;

        public StandardLocations()
        {
            ResetToDefault();
        }

        public string GetPathTo(Location location)
        {
            if (_paths.ContainsKey(location))
            {
                var path = _paths[location];

                if (!string.IsNullOrEmpty(path))
                {
                    path = path.Replace('\\', Separator);
                    path = path.Replace('/', Separator);
                    path = Path.TrimEndingDirectorySeparator(path);
                }

                return path;
            }

            return null;
        }

        public bool IsPathEditable(Location location)
        {
            return _editablePaths.Contains(location);
        }

        public bool IsPathSet(Location location)
        {
            return !string.IsNullOrEmpty(GetPathTo(location));
        }

        public bool SetPathTo(Location location, string path)
        {
            if (IsPathEditable(location))
            {
                ForcePathTo(location, path);

                return true;
            }

            return false;
        }

        protected void ResetToDefault()
        {
            ForcePathTo(Location.ApplicationExecutable, Assembly.GetEntryAssembly().Location);
            ForcePathTo(Location.ApplicationDirectory, Path.GetDirectoryName(GetPathTo(Location.ApplicationExecutable)));
            ForcePathTo(Location.PluginsDirectory, Path.Combine(GetPathTo(Location.ApplicationDirectory), "Plugins"));
            ForcePathTo(Location.SettingsFile, Path.Combine(GetPathTo(Location.ApplicationDirectory), "Settings.xml"));
        }

        protected void ForcePathTo(Location location, string path)
        {
            _paths[location] = path;
        }
    }
}
