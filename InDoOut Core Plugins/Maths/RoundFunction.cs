using InDoOut_Core.Entities.Functions;
using System;

namespace InDoOut_Core_Plugins.Maths
{
    public class RoundFunction : AbstractSingularArithmeticFunction
    {
        private readonly Property<int> _decimalPlaces = new Property<int>("Decimal places", "The number of decimal places to round to.", true, 0);

        public override string Description => "Rounds a given value to the nearest number and decimal place.";

        public override string Name => "Round";

        public override string[] Keywords => new[] { "Rounding", "Even", "Whole"};

        protected override string Verb => "round";

        public RoundFunction() : base()
        {
            _ = AddProperty(_decimalPlaces);
        }

        protected override double Calculate(double number)
        {
            return Math.Round(number, _decimalPlaces.FullValue);
        }
    }
}
