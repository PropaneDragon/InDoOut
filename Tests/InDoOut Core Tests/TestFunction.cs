using InDoOut_Core.Entities.Functions;
using System;

namespace InDoOut_Core_Tests
{
    class TestFunction : Function
    {
        public IInput LastInput { get; private set; } = null;
        public IOutput OutputToTrigger { get; set; } = null;
        public Action Action { get; private set; } = null;
        public bool HasRun = false;

        private TestFunction()
        {
        }

        public TestFunction(Action action) : this()
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
