using InDoOut_Core.Entities.Functions;
using System;

namespace InDoOut_Core_Tests
{
    public class TestFunction : Function
    {
        public IInput LastInput { get; private set; } = null;
        public IOutput OutputToTrigger { get; set; } = null;
        public IInput InputToBuild { get; set; } = null;
        public IOutput OutputToBuild { get; set; } = null;
        public Action Action { get; set; } = null;
        public bool HasRun { get; set; }

        public override string Description => "Test function";

        public override string Name => "Test function";

        public override string Group => "Test";

        public override string[] Keywords => new[] { "" };

        public TestFunction()
        {
        }

        public TestFunction(Action action) : this()
        {
            Action = action;
        }

        public IInput CreateInputPublic(string name = "Input") => CreateInput(name);
        public IOutput CreateOutputPublic(string name = "Output", OutputType outputType = OutputType.Neutral) => CreateOutput(name, outputType);
        public IOutput CreateOutputPublic(OutputType outputType, string name = "Output") => CreateOutput(name, outputType);
        public T AddPropertyPublic<T>(T property) where T : IProperty => AddProperty<T>(property);
        public T AddResultPublic<T>(T result) where T : IResult => AddResult<T>(result);

        protected override IOutput Started(IInput triggeredBy)
        {
            LastInput = triggeredBy;

            if (Action != null)
            {
                Action.Invoke();
            }

            HasRun = true;

            return OutputToTrigger;
        }

        protected override IInput BuildInput(string name)
        {
            return InputToBuild == null ? base.BuildInput(name) : InputToBuild;
        }

        protected override IOutput BuildOutput(string name, OutputType outputType)
        {
            return OutputToBuild == null ? base.BuildOutput(name, outputType) : OutputToBuild;
        }
    }
}
