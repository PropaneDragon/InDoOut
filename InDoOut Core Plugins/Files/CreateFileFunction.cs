using InDoOut_Core.Entities.Functions;
using System.IO;

namespace InDoOut_Core_Plugins.Files
{
    public class CreateFileFunction : Function
    {
        private readonly IOutput _created, _error;
        private readonly IProperty<string> _path, _contents;

        public override string Description => "Creates a text file at the specified location.";

        public override string Name => "Create a file";

        public override string Group => "Files";

        public override string[] Keywords => new[] { "text", "dir", "directory", "files", "txt" };

        public override IOutput TriggerOnFailure => _error;

        public CreateFileFunction()
        {
            _ = CreateInput("Create file");

            _created = CreateOutput("Created", OutputType.Positive);
            _error = CreateOutput("Error", OutputType.Negative);

            _path = AddProperty(new Property<string>("Path", "The path (including file name and extension) to create the file at.", true));
            _contents = AddProperty(new Property<string>("Contents", "The contents to put into the file when it is created.", true));
        }

        protected override IOutput Started(IInput triggeredBy)
        {
            File.WriteAllText(_path.FullValue, _contents.FullValue);
            return _created;
        }
    }
}
