using InDoOut_Core.Instancing;

namespace InDoOut_Desktop.Location
{
    internal enum Location
    {
        PluginsDirectory,
        ApplicationExecutable,
        ApplicationDirectory,
        SettingsFile,
        SaveFile
    }

    internal interface IStandardLocations : ISingleton<IStandardLocations>
    {
        bool PathEditable(Location location);
        bool PathSet(Location location);
        bool SetPathTo(Location location, string path);
        string GetPathTo(Location location);
    }
}
