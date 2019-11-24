using InDoOut_Core.Entities.Functions;
using System;

namespace InDoOut_Core_Plugins.SystemInfo
{
    /*public */class GetTotalMemoryFunction : Function
    {
        readonly IOutput _output;
        readonly IResult _memoryBits;

        public override string Description => "Gets total memory in the current machine.";

        public override string Name => "Get total memory";

        public override string Group => "System";

        public override string[] Keywords => new string[] { "ram" };

        public override IOutput TriggerOnFailure => _output;

        public GetTotalMemoryFunction()
        {
            _ = CreateInput("Get memory");

            _output = CreateOutput();

            _memoryBits = AddResult(new Result("Total memory (bits)", "Total bits of memory present in the machine", "0"));
        }

        protected override IOutput Started(IInput triggeredBy)
        {
            _memoryBits.ValueFrom(Environment.WorkingSet);

            return _output;
        }
    }
}
