using InDoOut_Core.Entities.Functions;
using System.IO;

namespace InDoOut_Core_Plugins.Paths
{
    public class GetDirectoryFunction : Function
    {
        private readonly IOutput _output, _failed;
        private readonly IProperty<string> _path;
        private readonly IResult _directoryPath;

        public override string Description => "Gets the directory path for any given path.";

        public override string Name => "Get directory path";

        public override string Group => "Paths";

        public override string[] Keywords => new[] { "folder" };

        public override IOutput TriggerOnFailure => _failed;

        public GetDirectoryFunction()
        {
            _ = CreateInput("Find path");

            _output = CreateOutput("Path found", OutputType.Positive);
            _failed = CreateOutput("Invalid path", OutputType.Negative);

            _path = AddProperty(new Property<string>("Path", "The path to get the directory for."));

            _directoryPath = AddResult(new Result("Directory path", "The path of the directory from the given path."));
        }

        protected override IOutput Started(IInput triggeredBy) => _directoryPath.ValueFrom(Path.GetDirectoryName(_path.FullValue)) ? _output : _failed;
    }
}
