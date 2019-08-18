using InDoOut_Core.Entities.Functions;
using System;

namespace InDoOut_Core_Plugins.Time
{
    public class CurrentTimeFunction : Function
    {
        private IInput _timeUtc;
        private IOutput _output;

        private Result _hour = new Result("Hour", "The current hour.");
        private Result _minute = new Result("Minute", "The current minute.");
        private Result _second = new Result("Second", "The current second.");
        private Result _millisecond = new Result("Millisecond", "The current millisecond.");

        public override string Description => "Gets the current time.";

        public override string Name => "Current time";

        public override string Group => "Time";

        public override string[] Keywords => new[] { "time", "hour", "minute", "second", "date", "datetime" };

        public CurrentTimeFunction() : base()
        {
            _ = CreateInput("Get current time");
            _timeUtc = CreateInput("Get current UTC time");

            _output = CreateOutput(OutputType.Neutral);

            _ = AddResult(_hour);
            _ = AddResult(_minute);
            _ = AddResult(_second);
            _ = AddResult(_millisecond);
        }

        protected override IOutput Started(IInput triggeredBy)
        {
            var usingUtc = triggeredBy == _timeUtc;
            var currentDate = usingUtc ? DateTime.UtcNow : DateTime.Now;

            _hour.ValueFrom(currentDate.Hour);
            _minute.ValueFrom(currentDate.Minute);
            _second.ValueFrom(currentDate.Second);
            _millisecond.ValueFrom(currentDate.Millisecond);

            return _output;
        }
    }
}
