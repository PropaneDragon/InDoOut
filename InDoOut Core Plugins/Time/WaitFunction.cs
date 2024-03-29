﻿using InDoOut_Core.Entities.Functions;
using System;
using System.Diagnostics;
using System.Threading;

namespace InDoOut_Core_Plugins.Time
{
    public class WaitFunction : Function
    {
        private readonly IOutput _output;

        private readonly Property<int> _milliseconds = new("Milliseconds", "The total number of milliseconds to wait for.", true, 0);
        private readonly Property<int> _seconds = new("Seconds", "The total number of seconds to wait for.", true, 1);
        private readonly Property<int> _minutes = new("Minutes", "The total number of minutes to wait for.", true, 0);
        private readonly Property<int> _hours = new("Hours", "The total number of hours to wait for.", true, 0);
        private readonly Property<int> _days = new("Days", "The total number of days to wait for.", true, 0);

        public override string Description => "Waits for a given time then continues.";

        public override string Name => "Wait";

        public override string Group => "Time";

        public override string[] Keywords => new[] { "pause", "hang", "stop", "timeout", "time", "timer", "count" };

        public WaitFunction() : base()
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
            var stopwatch = new Stopwatch();
            var endTime = new TimeSpan(_days.FullValue, _hours.FullValue, _minutes.FullValue, _seconds.FullValue, _milliseconds.FullValue);

            stopwatch.Start();

            while (!StopRequested && stopwatch.Elapsed < endTime)
            {
                Thread.Sleep(TimeSpan.FromMilliseconds(1));
            }

            return _output;
        }
    }
}
