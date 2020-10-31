using InDoOut_Core.Entities.Functions;
using System.IO;

namespace InDoOut_Core_Plugins.Files
{
    public class CheckFileExistsFunction : Function
    {
        private readonly IOutput _exists, _doesntExist;
        private readonly IProperty<string> _fileLocation;

        public override string Description => "Checks whether a file exists or not.";

        public override string Name => "Check file exists";

        public override string Group => "Files";

        public override string[] Keywords => new[] { "verify", "on disk" };

        public CheckFileExistsFunction() : base()
        {
            _ = CreateInput("Check file");

            _exists = CreateOutput("File exists", OutputType.Positive);
            _doesntExist = CreateOutput("Read failed", OutputType.Negative);

            _fileLocation = AddProperty(new Property<string>("File location", "The location and file name of the file to find.", true));
        }

        protected override IOutput Started(IInput triggeredBy) => (!string.IsNullOrEmpty(_fileLocation.FullValue) && File.Exists(_fileLocation.FullValue)) ? _exists : _doesntExist;
    }
}
