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
    public class RunIDOProgramFunction : Function
    {
        private static readonly int MAX_WAIT_COUNT = 3;

        private readonly IOutput _completed, _failed;
        private readonly IProperty<string> _programPath;
        private readonly List<IProperty<string>> _inputValues = new List<IProperty<string>>();
        private readonly IResult _result;

        public override IOutput TriggerOnFailure => _failed;

        public override string Description => "Runs an IDO program internally, allowing for programs to be used as self-contained functions.";

        public override string Name => "Run IDO program";

        public override string Group => "Self";

        public override string[] Keywords => new[] { "function", "run", "runner", "selfaware", "aware" };

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
                    var loader = new ProgramJsonStorer(builder, plugins, _programPath.FullValue);
                    var program = new Program(_inputValues.Select(value => value.FullValue).ToArray());
                    var loadResult = loader.Load(program);

                    if (loadResult.Count == 0 && program.StartFunctions.Count > 0)
                    {
                        program.Trigger(this);

                        var waitCount = 0;
                        while(waitCount < MAX_WAIT_COUNT)
                        {
                            if (program.Running)
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

                        return _completed;
                    }
                }
            }

            return _failed;
        }
    }
}
