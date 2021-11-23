using InDoOut_Core.Entities.Functions;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace InDoOut_Core_Plugins.Files
{
    public class ForEachFileFunction : LoopFunction
    {
        private readonly IProperty<string> _propertyDirectoryPath = null;
        private readonly IResult _resultFilePath = null;

        private List<string> _files = new();

        public override string Description => "Loops through all files within a specific directory.";

        public override string Name => "For each file";

        public override string Group => "Files";

        public override string[] Keywords => new[] { "foreach", "file", "files", "text", "filesystem", "dir", "loop" };

        public ForEachFileFunction()
        {
            _propertyDirectoryPath = AddProperty(new Property<string>("Search directory path", "The directory to search through.", false, ""));
            _resultFilePath = AddResult(new Result("Found file path", "The full path to the found file within the search directory."));
        }

        protected override void PreprocessItems()
        {
            _files.Clear();

            if (!string.IsNullOrEmpty(_propertyDirectoryPath.FullValue) && Directory.Exists(_propertyDirectoryPath.FullValue))
            {
                _files = Directory.GetFiles(_propertyDirectoryPath.FullValue).ToList();
            }
        }

        protected override bool PopulateItemDataForIndex(int index)
        {
            if (index < _files.Count)
            {
                _resultFilePath.RawValue = _files[index];

                return true;
            }

            return false;
        }

        protected override void AllItemsComplete() => _files.Clear();
    }
}
