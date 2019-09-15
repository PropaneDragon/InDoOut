using InDoOut_Core.Entities.Functions;
using System.Diagnostics;
using System.IO;

namespace InDoOut_Core_Plugins.Process
{
    public class StartProcessFunction : Function
    {
        private readonly IOutput _outputExecuted, _outputFailed;
        private readonly IProperty<string> _propertyLocation, _propertyArguments, _startInFolder;
        private readonly IResult _returnCode;

        public override string Description => "Starts a process on the device.";

        public override string Name => "Start process";

        public override string Group => "Process";

        public override string[] Keywords => new[] { "start", "process", "execute", "begin", "launch", "console", "terminal" };

        public StartProcessFunction()
        {
            _ = CreateInput("Start process");

            _outputExecuted = CreateOutput("Executed", OutputType.Positive);
            _outputFailed = CreateOutput("Failed", OutputType.Negative);
            _propertyLocation = AddProperty(new Property<string>("Process location", "The location of the process to execute.", true, ""));
            _propertyArguments = AddProperty(new Property<string>("Process arguments", "Arguments to pass to the executable.", false, ""));
            _startInFolder = AddProperty(new Property<string>("Working directory", "Starts the process from the given location.", false, ""));
            _returnCode = AddResult(new Result("Return code", "The value returned from the process after execution.", "0"));
        }

        protected override IOutput Started(IInput triggeredBy)
        {
            if (_propertyLocation.FullValue != null && File.Exists(_propertyLocation.FullValue))
            {
                try
                {
                    using var process = new System.Diagnostics.Process
                    {
                        StartInfo = new ProcessStartInfo(_propertyLocation.FullValue, _propertyArguments.FullValue)
                        {
                            CreateNoWindow = false,
                            UseShellExecute = false,
                            WorkingDirectory = _startInFolder.FullValue ?? ""
                        }
                    };

                    if (process.Start())
                    {
                        process.WaitForExit();

                        _ = _returnCode.ValueFrom(process.ExitCode);

                        return _outputExecuted;
                    }
                }
                catch { }
            }

            return _outputFailed;
        }
    }
}
