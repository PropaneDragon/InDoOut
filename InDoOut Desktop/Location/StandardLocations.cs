using InDoOut_Core.Instancing;
using InDoOut_Executable_Core.Location;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace InDoOut_Desktop.Location
{
    internal class StandardLocations : Singleton<StandardLocations>, IStandardLocations
    {
        private Dictionary<InDoOut_Executable_Core.Location.Location, string> _paths = new Dictionary<InDoOut_Executable_Core.Location.Location, string>();

        private List<InDoOut_Executable_Core.Location.Location> _editablePaths = new List<InDoOut_Executable_Core.Location.Location>()
        {
            InDoOut_Executable_Core.Location.Location.SaveFile,
            InDoOut_Executable_Core.Location.Location.PluginsDirectory,
            InDoOut_Executable_Core.Location.Location.SettingsFile
        };

        public static char Separator => Path.DirectorySeparatorChar;

        public StandardLocations()
        {
            ResetToDefault();
        }

        public string GetPathTo(InDoOut_Executable_Core.Location.Location location)
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

        public bool PathEditable(InDoOut_Executable_Core.Location.Location location)
        {
            return _editablePaths.Contains(location);
        }

        public bool PathSet(InDoOut_Executable_Core.Location.Location location)
        {
            return !string.IsNullOrEmpty(GetPathTo(location));
        }

        public bool SetPathTo(InDoOut_Executable_Core.Location.Location location, string path)
        {
            if (PathEditable(location))
            {
                ForcePathTo(location, path);

                return true;
            }

            return false;
        }

        protected void ResetToDefault()
        {
            ForcePathTo(InDoOut_Executable_Core.Location.Location.ApplicationExecutable, Assembly.GetEntryAssembly().Location);
            ForcePathTo(InDoOut_Executable_Core.Location.Location.ApplicationDirectory, Path.GetDirectoryName(GetPathTo(InDoOut_Executable_Core.Location.Location.ApplicationExecutable)));
            ForcePathTo(InDoOut_Executable_Core.Location.Location.PluginsDirectory, Path.Combine(GetPathTo(InDoOut_Executable_Core.Location.Location.ApplicationDirectory), "Plugins"));
            ForcePathTo(InDoOut_Executable_Core.Location.Location.SettingsFile, Path.Combine(GetPathTo(InDoOut_Executable_Core.Location.Location.ApplicationDirectory), "Settings.xml"));
        }

        protected void ForcePathTo(InDoOut_Executable_Core.Location.Location location, string path)
        {
            _paths[location] = path;
        }
    }
}
