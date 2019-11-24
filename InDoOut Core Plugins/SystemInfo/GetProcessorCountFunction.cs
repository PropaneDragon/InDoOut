using InDoOut_Core.Entities.Functions;
using System;

namespace InDoOut_Core_Plugins.SystemInfo
{
    public class GetProcessorCountFunction : Function
    {
        readonly IOutput _output;
        readonly IResult _processorCount;

        public override string Description => "Returns the number of logical processors present in the machine.";

        public override string Name => "Get processor count";

        public override string Group => "System";

        public override string[] Keywords => new string[] { "cpu", "virtual", "hyper-threading", "hyper", "threading" };

        public override IOutput TriggerOnFailure => _output;

        public GetProcessorCountFunction()
        {
            _ = CreateInput("Get processor count");

            _output = CreateOutput();

            _processorCount = AddResult(new Result("Processor count", "The number of processors in the machine", "0"));
        }

        protected override IOutput Started(IInput triggeredBy)
        {
            _processorCount.ValueFrom(Environment.ProcessorCount);

            return _output;
        }
    }
}
