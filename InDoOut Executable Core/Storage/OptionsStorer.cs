using System.Collections.Generic;
using InDoOut_Core.Logging;
using InDoOut_Core.Options;
using InDoOut_Core.Reporting;

namespace InDoOut_Executable_Core.Storage
{
    public abstract class OptionsStorer : IOptionsStorer
    {
        public abstract string FileExtension { get; }
        public string FilePath { get; set; }

        public OptionsStorer()
        {
        }

        public OptionsStorer(string filePath) : this()
        {
            FilePath = filePath;
        }

        public List<IFailureReport> Load(IOptionHolder optionHolder)
        {
            Log.Instance.Header("Attempting to load");
            Log.Instance.Info("Attempting to load options from ", FilePath);

            var reports = TryLoad(optionHolder, FilePath);

            Log.Instance.Header("Attempt to load complete");

            return reports;
        }

        public List<IFailureReport> Save(IOptionHolder optionHolder)
        {
            Log.Instance.Header("Attempting to save");
            Log.Instance.Info("Attempting to save options to ", FilePath);

            var reports = TrySave(optionHolder, FilePath);

            Log.Instance.Header("Attempt to save completed");

            return reports;
        }

        protected abstract List<IFailureReport> TryLoad(IOptionHolder optionHolder, string path);
        protected abstract List<IFailureReport> TrySave(IOptionHolder optionHolder, string path);
    }
}
