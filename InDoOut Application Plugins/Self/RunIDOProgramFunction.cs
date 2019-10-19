using InDoOut_Core.Entities.Functions;
using InDoOut_Core.Entities.Programs;
using InDoOut_Core.Functions;
using InDoOut_Json_Storage;
using InDoOut_Plugins.Loaders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace InDoOut_Application_Plugins.Self
{
    public class RunIDOProgramFunction : SelfRunnerFunction
    {
        private static readonly int MAX_WAIT_COUNT = 100;

        private readonly IOutput _completed, _failed;
        private readonly IProperty<string> _programPath;
        private readonly IResult _result;
        private readonly List<IProperty<string>> _inputValues = new List<IProperty<string>>();

        private IProgram _currentProgram = null;

        public override IOutput TriggerOnFailure => _failed;

        public override string Description => "Runs an IDO program internally, allowing for programs to be used as self-contained functions.";

        public override string Name => "Run IDO program";

        public override string Group => "Self";

        public override string[] Keywords => new[] { "function", "run", "runner", "selfaware", "aware" };

        public override IProgram LoadedProgram => _currentProgram;

        public RunIDOProgramFunction()
        {
            _ = CreateInput("Start");

            _completed = CreateOutput("Completed", OutputType.Positive);
            _failed = CreateOutput("Failed", OutputType.Negative);

            _programPath = AddProperty(new Property<string>("Path", "The full path to the IDO program.", true));

            _result = AddResult(new Result("Result", "The result value from the program."));

            for (var count = 1; count <= StartFunction.TOTAL_OUTPUTS; ++count)
            {
                _inputValues.Add(AddProperty(new Property<string>($"Value {count}", $"Value number {count} to pass into the program.")));
            }
        }

        protected override IOutput Started(IInput triggeredBy)
        {
            if (!string.IsNullOrEmpty(_programPath?.FullValue) && File.Exists(_programPath.FullValue))
            {
                var plugins = LoadedPlugins.Instance;
                var builder = new FunctionBuilder();

                if (plugins != null)
                {
                    _currentProgram = new Program(_inputValues.Select(value => value.FullValue).ToArray());

                    var loader = new ProgramJsonStorer(builder, plugins, _programPath.FullValue);
                    var loadResult = loader.Load(_currentProgram);

                    if (loadResult.Count == 0 && _currentProgram.StartFunctions.Count > 0)
                    {
                        _currentProgram.Trigger(this);

                        var waitCount = 0;
                        while (waitCount < MAX_WAIT_COUNT)
                        {
                            if (StopRequested && !_currentProgram.Stopping)
                            {
                                _currentProgram.Stop();
                            }

                            if (_currentProgram.Running || _currentProgram.Stopping)
                            {
                                waitCount = 0;
                            }
                            else
                            {
                                ++waitCount;
                            }

                            Thread.Sleep(TimeSpan.FromMilliseconds(10));
                        }

                        _result.RawValue = ""; //Todo: Add result from program when implemented.

                        _currentProgram.Stop();
                        _currentProgram = null;

                        loader = null;
                        builder = null;

                        return _completed;
                    }
                }
            }

            return _failed;
        }
    }
}
