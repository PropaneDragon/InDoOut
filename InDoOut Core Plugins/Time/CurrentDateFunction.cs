using InDoOut_Core.Entities.Functions;
using System;

namespace InDoOut_Core_Plugins.Time
{
    public class CurrentDateFunction : Function
    {
        private readonly IInput _timeUtc;
        private readonly IOutput _output;

        private readonly Result _year = new Result("Year", "The current year.");
        private readonly Result _month = new Result("Month", "The current month.");
        private readonly Result _day = new Result("Day", "The current day.");
        private readonly Result _dayOfYear = new Result("Day of year", "The current day of the year.");
        private readonly Result _dayOfWeek = new Result("Day of week", "The current day of the week.");

        public override string Description => "Gets the current date.";

        public override string Name => "Current date";

        public override string Group => "Time";

        public override string[] Keywords => new[] { "date", "day", "year", "month", "time", "datetime" };

        public CurrentDateFunction() : base()
        {
            _ = CreateInput("Get current date");
            _timeUtc = CreateInput("Get current UTC date");

            _output = CreateOutput(OutputType.Neutral);

            _ = AddResult(_year);
            _ = AddResult(_month);
            _ = AddResult(_day);
            _ = AddResult(_dayOfYear);
            _ = AddResult(_dayOfWeek);
        }

        protected override IOutput Started(IInput triggeredBy)
        {
            var usingUtc = triggeredBy == _timeUtc;
            var currentDate = usingUtc ? DateTime.UtcNow : DateTime.Now;

            _year.ValueFrom(currentDate.Year);
            _month.ValueFrom(currentDate.Month);
            _day.ValueFrom(currentDate.Day);
            _dayOfYear.ValueFrom(currentDate.DayOfYear);
            _dayOfWeek.ValueFrom((int)currentDate.DayOfWeek);

            return _output;
        }
    }
}
