﻿using InDoOut_Core.Entities.Functions;
using System.IO;

namespace InDoOut_Core_Plugins.Files
{
    public class ReadFileFunction : Function
    {
        private readonly IOutput _success, _failure;
        private readonly IProperty<string> _fileLocation;
        private readonly IResult _fileContents;

        public override string Description => "Reads the contents of a file.";

        public override string Name => "Read file";

        public override string Group => "Files";

        public override string[] Keywords => new[] { "read", "contents", "text", "all", "load", "open", "string" };

        public ReadFileFunction() : base()
        {
            _ = CreateInput("Read file");

            _success = CreateOutput("File read", OutputType.Positive);
            _failure = CreateOutput("Read failed", OutputType.Negative);

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
