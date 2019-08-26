using InDoOut_Core.Entities.Functions;
using System;
using System.Collections.Generic;

namespace InDoOut_Core_Plugins.Time
{
    public class CurrentDayFunction : Function
    {
        private readonly Dictionary<DayOfWeek, IOutput> _outputs = new Dictionary<DayOfWeek, IOutput>();

        public override string Description => "Triggers the current day.";

        public override string Name => "Current day";

        public override string Group => "Time";

        public override string[] Keywords => new[] { "date", "day", "time", "datetime", "current" };

        public CurrentDayFunction() : base()
        {
            _ = CreateInput("Get current day");

            PopulateOutputs();
        }

        protected override IOutput Started(IInput triggeredBy)
        {
            var usingUtc = false; //triggeredBy == _timeUtc;
            var currentDate = usingUtc ? DateTime.UtcNow : DateTime.Now;
            var currentDay = currentDate.DayOfWeek;

            return _outputs[currentDay];
        }

        private void PopulateOutputs()
        {
            foreach (DayOfWeek day in Enum.GetValues(typeof(DayOfWeek)))
            {
                _outputs.Add(day, CreateOutput(day.ToString()));
            }
        }
    }
}
