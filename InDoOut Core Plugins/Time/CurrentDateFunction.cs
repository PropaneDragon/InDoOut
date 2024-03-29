﻿using InDoOut_Core.Entities.Functions;
using System;

namespace InDoOut_Core_Plugins.Time
{
    public class CurrentDateFunction : Function
    {
        private readonly IInput _timeUtc;
        private readonly IOutput _output;

        private readonly Result _year = new("Year", "The current year.");
        private readonly Result _month = new("Month", "The current month.");
        private readonly Result _day = new("Day", "The current day.");
        private readonly Result _dayOfYear = new("Day of year", "The current day of the year.");
        private readonly Result _dayOfWeek = new("Day of week", "The current day of the week.");

        public override string Description => "Gets the current date.";

        public override string Name => "Current date";

        public override string Group => "Time";

        public override string[] Keywords => new[] { "date", "day", "year", "month", "time", "datetime" };

        public CurrentDateFunction() : base()
        {
            _ = CreateInput("Get current date");
            _timeUtc = CreateInput("Get current UTC date");

            _output = CreateOutput("Date retrieved", OutputType.Neutral);

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

            _ = _year.ValueFrom(currentDate.Year);
            _ = _month.ValueFrom(currentDate.Month);
            _ = _day.ValueFrom(currentDate.Day);
            _ = _dayOfYear.ValueFrom(currentDate.DayOfYear);
            _ = _dayOfWeek.ValueFrom((int)currentDate.DayOfWeek);

            return _output;
        }
    }
}
