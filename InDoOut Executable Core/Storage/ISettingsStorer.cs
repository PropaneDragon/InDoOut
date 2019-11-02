using InDoOut_Core.Plugins;
using InDoOut_Core.Reporting;
using System.Collections.Generic;

namespace InDoOut_Executable_Core.Storage
{
    public interface ISettingsStorer
    {
        string FileExtension { get; }

        string FilePath { get; set; }

        List<IFailureReport> Save(IPlugin plugin);

        List<IFailureReport> Load(IPlugin plugin);
    }
}
