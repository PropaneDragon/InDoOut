using System;

namespace InDoOut_Core_Plugins.Maths
{
    public class CeilingFunction : AbstractSingularArithmeticFunction
    {
        public override string Description => "Rounds a given value up.";

        public override string Name => "Ceiling";

        public override string[] Keywords => new[] { "Rounding", "Round up", "up", "highest", "top"};

        protected override string Verb => "ceiling";

        public CeilingFunction() : base()
        {
        }

        protected override double Calculate(double number) => Math.Ceiling(number);
    }
}
