using InDoOut_Core.Options;
using InDoOut_Core.Reporting;
using System.Collections.Generic;

namespace InDoOut_Executable_Core.Storage
{
    public interface IOptionsStorer
    {
        string FileExtension { get; }

        string FilePath { get; set; }

        List<IFailureReport> Save(IOptionHolder optionHolder);

        List<IFailureReport> Load(IOptionHolder optionHolder);
    }
}
