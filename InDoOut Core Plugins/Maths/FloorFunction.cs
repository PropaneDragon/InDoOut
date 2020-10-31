using System;

namespace InDoOut_Core_Plugins.Maths
{
    public class FloorFunction : AbstractSingularArithmeticFunction
    {
        public override string Description => "Rounds a given value down.";

        public override string Name => "Floor";

        public override string[] Keywords => new[] { "Rounding", "Round down", "down", "lowest", "bottom"};

        protected override string Verb => "floor";

        public FloorFunction() : base()
        {
        }

        protected override double Calculate(double number) => Math.Floor(number);
    }
}
