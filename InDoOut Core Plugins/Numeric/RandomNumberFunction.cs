using InDoOut_Core.Entities.Functions;
using System;

namespace InDoOut_Core_Plugins.Numeric
{
    public class RandomNumberFunction : Function
    {
        private readonly IOutput _output = null;
        private readonly IProperty<int> _propertyMinimum = null;
        private readonly IProperty<int> _propertyMaximum = null;
        private readonly IResult _resultNumber = null;

        private readonly Random _random = new Random();

        public override string Description => "Generates a random number from a minimum and maximum value.";

        public override string Name => "Random number";

        public override string Group => "Numeric";

        public override string[] Keywords => new[] { "rand", "random", "randy" };

        public RandomNumberFunction()
        {
            _ = CreateInput("Generate random number");
            _output = CreateOutput("Generated");
            _propertyMinimum = AddProperty(new Property<int>("Minimum number (inclusive)", "The minimum (non-fractional) value to generate.", true, 0));
            _propertyMaximum = AddProperty(new Property<int>("Maximum number (exclusive)", "The maximum (non-fractional) value to generate (exclusive).", true, 100));
            _resultNumber = AddResult(new Result("Generated number", "The randomly generated number."));
        }

        protected override IOutput Started(IInput triggeredBy)
        {
            _ = _resultNumber.ValueFrom(_random.Next(_propertyMinimum.FullValue, _propertyMaximum.FullValue));

            return _output;
        }
    }
}
