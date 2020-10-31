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

        public ProgramStorer()
        {
        }

        public List<IFailureReport> Load(IProgram program, Stream stream)
        {
            Log.Instance.Header("Attempting to load");
            Log.Instance.Info("Attempting to load ", program);

            var reports = TryLoad(program, stream);

            Log.Instance.Info($"Load {(reports.Any(report => report.Critical) ? "failed" : "completed")} with {reports.Count} issues, {reports.Where(report => report.Critical).Count()} of which are critical");

            foreach (var report in reports)
            {
                Log.Instance.Error(report);
            }

            Log.Instance.Header("Attempt to load completed");

            return reports;
        }

        public List<IFailureReport> Save(IProgram program, Stream stream)
        {
            Log.Instance.Header("Attempting to save");
            Log.Instance.Info("Attempting to save ", program);

            var reports = TrySave(program, stream);

            Log.Instance.Info($"Save {(reports.Any(report => report.Critical) ? "failed" : "completed")} with {reports.Count} issues, {reports.Where(report => report.Critical).Count()} of which are critical");

            foreach (var report in reports)
            {
                Log.Instance.Error(report);
            }

            Log.Instance.Header("Attempt to save completed");

            return reports;
        }

        protected abstract List<IFailureReport> TryLoad(IProgram program, Stream stream);
        protected abstract List<IFailureReport> TrySave(IProgram program, Stream stream);
    }
}
