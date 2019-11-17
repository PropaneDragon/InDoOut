using InDoOut_Core.Entities.Functions;
using System.IO;

namespace InDoOut_Core_Plugins.Files
{
    public class AppendFileFunction : Function
    {
        private readonly IOutput _appended, _error;
        private readonly IProperty<string> _path, _contents;

        public override string Description => "Appends text to the end of a file at the specified location.";

        public override string Name => "Append file";

        public override string Group => "Files";

        public override string[] Keywords => new[] { "text", "dir", "directory", "files", "txt", "add", "concatenate", "last" };

        public override IOutput TriggerOnFailure => _error;

        public AppendFileFunction()
        {
            _ = CreateInput("Append file");

            _appended = CreateOutput("Appended", OutputType.Positive);
            _error = CreateOutput("Error", OutputType.Negative);

            _path = AddProperty(new Property<string>("Path", "The path of the file to append text to.", true));
            _contents = AddProperty(new Property<string>("Contents", "The contents to append to the end of the file.", true));
        }

        protected override IOutput Started(IInput triggeredBy)
        {
            File.AppendAllText(_path.FullValue, _contents.FullValue);
            return _appended;
        }
    }
}
