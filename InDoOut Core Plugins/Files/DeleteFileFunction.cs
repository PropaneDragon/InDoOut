using InDoOut_Core.Entities.Functions;
using System.IO;

namespace InDoOut_Core_Plugins.Files
{
    public class DeleteFileFunction : Function
    {
        private readonly IOutput _success, _failure;
        private readonly IProperty<string> _fileLocation;

        public override string Description => "Deletes a file from the drive.";

        public override string Name => "Delete file";

        public override string Group => "Files";

        public override string[] Keywords => new[] { "remove", "clear", "uninstall", "rm", "cls" };

        public DeleteFileFunction() : base()
        {
            _ = CreateInput("Delete file");

            _success = CreateOutput("File deleted", OutputType.Positive);
            _failure = CreateOutput("Deletion failed", OutputType.Negative);

            _fileLocation = AddProperty(new Property<string>("File location", "The location and file name of the file to delete.", true));
        }

        protected override IOutput Started(IInput triggeredBy)
        {
            if (!string.IsNullOrEmpty(_fileLocation.FullValue) && File.Exists(_fileLocation.FullValue))
            {
                try
                {
                    File.Delete(_fileLocation.FullValue);

                    return _success;
                }
                catch { }
            }

            return _failure;
        }
    }
}
