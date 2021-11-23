using InDoOut_Core.Entities.Functions;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace InDoOut_Core_Plugins.Files
{
    public class ForEachDirectoryFunction : LoopFunction
    {
        private readonly IProperty<string> _propertyDirectoryPath = null;
        private readonly IResult _resultDirectoryPath = null;

        private List<string> _directories = new();

        public override string Description => "Loops through all directories within a specific directory.";

        public override string Name => "For each directory";

        public override string Group => "Files";

        public override string[] Keywords => new[] { "foreach", "directory", "directories", "file", "folders", "filesystem", "dir", "loop" };

        public ForEachDirectoryFunction()
        {
            _propertyDirectoryPath = AddProperty(new Property<string>("Search directory path", "The directory to search through.", false, ""));
            _resultDirectoryPath = AddResult(new Result("Found directory path", "The full path to the found directory within the search directory."));
        }

        protected override void PreprocessItems()
        {
            _directories.Clear();

            if (!string.IsNullOrEmpty(_propertyDirectoryPath.FullValue) && Directory.Exists(_propertyDirectoryPath.FullValue))
            {
                _directories = Directory.GetDirectories(_propertyDirectoryPath.FullValue).ToList();
            }
        }

        protected override bool PopulateItemDataForIndex(int index)
        {
            if (index < _directories.Count)
            {
                _resultDirectoryPath.RawValue = _directories[index];

                return true;
            }

            return false;
        }

        protected override void AllItemsComplete() => _directories.Clear();
    }
}
