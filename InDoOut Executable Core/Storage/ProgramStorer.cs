using System.Collections.Generic;
using System.IO;
using System.Linq;
using InDoOut_Core.Entities.Programs;
using InDoOut_Core.Logging;
using InDoOut_Core.Reporting;

namespace InDoOut_Executable_Core.Storage
{
    public abstract class ProgramStorer : IProgramStorer
    {
        public static readonly string PROGRAM_METADATA_LAST_LOADED_FROM = "lastLoadedFrom";

        public abstract string FileReadableName { get; }
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
            Log.Instance.Header("Attempting to load");
            Log.Instance.Info("Attempting to load ", program, " from ", FilePath);

            var reports = TryLoad(program, FilePath);

            program.Metadata[PROGRAM_METADATA_LAST_LOADED_FROM] = FilePath;
            program.SetName(Path.GetFileNameWithoutExtension(FilePath));

            Log.Instance.Info($"Load {(reports.Any(report => report.Critical) ? "failed" : "completed")} from ", FilePath, $" with {reports.Count} issues, {reports.Where(report => report.Critical).Count()} of which are critical");

            foreach (var report in reports)
            {
                Log.Instance.Error(report);
            }

            Log.Instance.Header("Attempt to load completed");

            return reports;
        }

        public List<IFailureReport> Save(IProgram program)
        {
            Log.Instance.Header("Attempting to save");
            Log.Instance.Info("Attempting to save ", program, " to ", FilePath);

            var reports = TrySave(program, FilePath);
            if (!reports.Any(report => report.Critical))
            {
                program.Metadata[PROGRAM_METADATA_LAST_LOADED_FROM] = FilePath;
                program.SetName(Path.GetFileNameWithoutExtension(FilePath));
            }

            Log.Instance.Info($"Save {(reports.Any(report => report.Critical) ? "failed" : "completed")} to ", FilePath, $" with {reports.Count} issues, {reports.Where(report => report.Critical).Count()} of which are critical");

            foreach (var report in reports)
            {
                Log.Instance.Error(report);
            }

            Log.Instance.Header("Attempt to save completed");

            return reports;
        }

        protected abstract List<IFailureReport> TryLoad(IProgram program, string path);
        protected abstract List<IFailureReport> TrySave(IProgram program, string path);
    }
}
