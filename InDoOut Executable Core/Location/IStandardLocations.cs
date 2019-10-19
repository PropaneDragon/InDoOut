using InDoOut_Core.Instancing;

namespace InDoOut_Executable_Core.Location
{
    public enum Location
    {
        PluginsDirectory,
        ApplicationExecutable,
        ApplicationDirectory,
        SettingsFile,
        SaveFile
    }

    public interface IStandardLocations : ISingleton<IStandardLocations>
    {
        bool IsPathEditable(Location location);
        bool IsPathSet(Location location);
        bool SetPathTo(Location location, string path);
        string GetPathTo(Location location);
    }
}
