using InDoOut_Core.Entities.Functions;
using System.IO;

namespace InDoOut_Core_Plugins.Paths
{
    public class GetFileNameWithoutExtensionFunction : Function
    {
        private readonly IOutput _output, _failed;
        private readonly IProperty<string> _path;
        private readonly IResult _fileName;

        public override string Description => "Gets the name of a file (without extension) from a path.";

        public override string Name => "Get file name without extension";

        public override string Group => "Paths";

        public override string[] Keywords => new[] { "files" };

        public override IOutput TriggerOnFailure => _failed;

        public GetFileNameWithoutExtensionFunction()
        {
            _ = CreateInput("Find file name");

            _output = CreateOutput("Name found", OutputType.Positive);
            _failed = CreateOutput("Invalid path", OutputType.Negative);

            _path = AddProperty(new Property<string>("Path", "The path to get the file name for."));

            _fileName = AddResult(new Result("File name", "The name of the file from the given path."));
        }

        protected override IOutput Started(IInput triggeredBy)
        {
            return _fileName.ValueFrom(Path.GetFileNameWithoutExtension(_path.FullValue)) ? _output : _failed;
        }
    }
}
