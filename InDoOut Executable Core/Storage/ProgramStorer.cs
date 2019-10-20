using System.Collections.Generic;
using System.IO;
using System.Linq;
using InDoOut_Core.Entities.Programs;
using InDoOut_Core.Reporting;

namespace InDoOut_Executable_Core.Storage
{
    public abstract class ProgramStorer : IProgramStorer
    {
        public static readonly string PROGRAM_METADATA_LAST_LOADED_FROM = "lastLoadedFrom";

        public abstract string FileExtension { get; }
        public string FilePath { get; set; }

        public ProgramStorer()
        {
        }

        public ProgramStorer(string filePath) : this()
        {
            FilePath = filePath;
        }

        public List<IFailureReport> Load(IProgram program)
        {
            var reports = TryLoad(program, FilePath);

            program.Metadata[PROGRAM_METADATA_LAST_LOADED_FROM] = FilePath;
            program.SetName(Path.GetFileNameWithoutExtension(FilePath));

            return reports;
        }

        public List<IFailureReport> Save(IProgram program)
        {
            var reports = TrySave(program, FilePath);
            if (!reports.Any(report => report.Critical))
            {
                program.Metadata[PROGRAM_METADATA_LAST_LOADED_FROM] = FilePath;
                program.SetName(Path.GetFileNameWithoutExtension(FilePath));
            }

            return reports;
        }

        protected abstract List<IFailureReport> TryLoad(IProgram program, string path);
        protected abstract List<IFailureReport> TrySave(IProgram program, string path);
    }
}
