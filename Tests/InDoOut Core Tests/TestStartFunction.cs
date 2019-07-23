using InDoOut_Core.Entities.Functions;
using System;

namespace InDoOut_Core_Tests
{
    public class TestStartFunction : StartFunction
    {
        public IInput LastInput { get; private set; } = null;
        public IOutput OutputToTrigger { get; set; } = null;
        public Action Action { get; private set; } = null;
        public bool HasRun { get; set; }

        public override string Description => "A test function";

        public override string Name => "Test start";

        public override string Group => "Test";

        public override string[] Keywords => new[] { "" };

        public TestStartFunction()
        {
        }

        public TestStartFunction(Action action) : this()
        {
            Action = action;
        }

        public IInput CreateInputPublic(string name = "Input") => CreateInput(name);
        public IOutput CreateOutputPublic(string name = "Output") => CreateOutput(name);

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
    }
}
