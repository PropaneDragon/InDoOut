using InDoOut_Core.Entities.Functions;
using System;

namespace InDoOut_Core_Plugins.Time
{
    public class CurrentTimeFunction : Function
    {
        private readonly IInput _timeUtc;
        private readonly IOutput _output;

        private readonly Result _hour = new Result("Hour", "The current hour.");
        private readonly Result _minute = new Result("Minute", "The current minute.");
        private readonly Result _second = new Result("Second", "The current second.");
        private readonly Result _millisecond = new Result("Millisecond", "The current millisecond.");

        public override string Description => "Gets the current time.";

        public override string Name => "Current time";

        public override string Group => "Time";

        public override string[] Keywords => new[] { "time", "hour", "minute", "second", "date", "datetime" };

        public CurrentTimeFunction() : base()
        {
            _ = CreateInput("Get current time");
            _timeUtc = CreateInput("Get current UTC time");

            _output = CreateOutput("Time retreived", OutputType.Neutral);

            _ = AddResult(_hour);
            _ = AddResult(_minute);
            _ = AddResult(_second);
            _ = AddResult(_millisecond);
        }

        protected override IOutput Started(IInput triggeredBy)
        {
            var usingUtc = triggeredBy == _timeUtc;
            var currentDate = usingUtc ? DateTime.UtcNow : DateTime.Now;

            _ = _hour.ValueFrom(currentDate.Hour);
            _ = _minute.ValueFrom(currentDate.Minute);
            _ = _second.ValueFrom(currentDate.Second);
            _ = _millisecond.ValueFrom(currentDate.Millisecond);

            return _output;
        }
    }
}
