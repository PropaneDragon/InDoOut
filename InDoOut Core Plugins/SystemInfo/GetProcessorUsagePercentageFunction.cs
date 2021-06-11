using InDoOut_Core.Entities.Functions;
using InDoOut_Core.Threading.Safety;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace InDoOut_Core_Plugins.SystemInfo
{
    internal class GetProcessorUsagePercentageFunction : Function
    {
        private readonly IOutput _output;
        private readonly IResult _cpuPercentage;

        public override string Description => "Gets current processor usage as a percentage. This gives a very limited usage view in non-administrator mode.";

        public override string Name => "Get current processor usage";

        public override string Group => "System";

        public override string[] Keywords => new string[] { "cpu", "computer", "time", "milliseconds", "ms" };

        public override IOutput TriggerOnFailure => _output;

        public GetProcessorUsagePercentageFunction()
        {
            _ = CreateInput("Get usage");

            _output = CreateOutput();

            _cpuPercentage = AddResult(new Result("Processor usage percentage", "The percentage of the processor currently being used", "0"));
        }

        protected override IOutput Started(IInput triggeredBy)
        {
            var stopwatch = new Stopwatch();
            var allProcesses = System.Diagnostics.Process.GetProcesses();

            stopwatch.Start();

            var totalUsageBefore = allProcesses.Select(process => TryGet.ValueOrDefault(() => process.TotalProcessorTime.TotalMilliseconds, 0)).Sum();

            Thread.Sleep(TimeSpan.FromMilliseconds(100));

            var totalUsageAfter = allProcesses.Select(process => TryGet.ValueOrDefault(() => process.TotalProcessorTime.TotalMilliseconds, 10)).Sum();

            stopwatch.Stop();

            var totalUsageBetween = totalUsageAfter - totalUsageBefore;
            var totalCpuUsage = Math.Clamp(totalUsageBetween / (Environment.ProcessorCount * stopwatch.ElapsedMilliseconds), 0d, 1d);

            _ = _cpuPercentage.ValueFrom(totalCpuUsage * 100d);

            return _output;
        }
    }
}
