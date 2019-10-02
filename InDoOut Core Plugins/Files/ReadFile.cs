using InDoOut_Core.Entities.Functions;
using System;
using System.IO;

namespace InDoOut_Core_Plugins.Files
{
    public class ReadFile : Function
    {
        private IOutput _success, _failure;
        private IProperty<string> _fileLocation;
        private IResult _fileContents;

        public override string Description => "Reads the contents of a file.";

        public override string Name => "Read file";

        public override string Group => "Files";

        public override string[] Keywords => new[] { "read", "contents", "text", "all", "load", "open", "string" };

        public ReadFile() : base()
        {
            _ = CreateInput("Read file");

            _success = CreateOutput("File read");
            _failure = CreateOutput("Read failed");

            _fileLocation = AddProperty(new Property<string>("File location", "The location and file name of the file to read.", true));

            _fileContents = AddResult(new Result("File contents", "The contents of the read file."));
        }

        protected override IOutput Started(IInput triggeredBy)
        {
            if (!string.IsNullOrEmpty(_fileLocation.FullValue) && File.Exists(_fileLocation.FullValue))
            {
                try
                {
                    var fileContents = File.ReadAllText(_fileLocation.FullValue);
                    if (_fileContents.ValueFrom(fileContents))
                    {
                        return _success;
                    }
                }
                catch { }
            }

            return _failure;
        }
    }
}
