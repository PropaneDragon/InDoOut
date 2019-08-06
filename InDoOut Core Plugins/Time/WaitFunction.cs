using InDoOut_Core.Entities.Functions;
using System;
using System.Threading.Tasks;

namespace InDoOut_Core_Plugins.Time
{
    public class Wait : Function
    {
        private IOutput _output;

        private Property<int> _milliseconds = new Property<int>("Milliseconds", "The total number of milliseconds to wait for.", true, 0);
        private Property<int> _seconds = new Property<int>("Seconds", "The total number of seconds to wait for.", true, 1);
        private Property<int> _minutes = new Property<int>("Minutes", "The total number of minutes to wait for.", true, 0);
        private Property<int> _hours = new Property<int>("Hours", "The total number of hours to wait for.", true, 0);
        private Property<int> _days = new Property<int>("Days", "The total number of days to wait for.", true, 0);

        public override string Description => "Waits for a given time then continues.";

        public override string Name => "Wait";

        public override string Group => "Time";

        public override string[] Keywords => new[] { "pause", "hang", "stop", "timeout", "time", "timer", "count" };

        public Wait() : base()
        {
            _ = CreateInput("Start waiting");

            _output = CreateOutput(OutputType.Neutral, "Wait complete");

            _ = AddProperty(_milliseconds);
            _ = AddProperty(_seconds);
            _ = AddProperty(_minutes);
            _ = AddProperty(_hours);
            _ = AddProperty(_days);
        }

        protected override IOutput Started(IInput triggeredBy)
        {
            var currentTime = DateTime.UtcNow;
            var endTime = currentTime + new TimeSpan(_days.FullValue, _hours.FullValue, _minutes.FullValue, _seconds.FullValue, _milliseconds.FullValue);

            while (!StopRequested && DateTime.UtcNow < endTime)
            {
                var waitTask = Task.Delay(TimeSpan.FromMilliseconds(1));
                waitTask.Wait();
            }

            return _output;
        }
    }
}
