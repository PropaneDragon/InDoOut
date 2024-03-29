﻿namespace InDoOut_Core_Plugins.Maths
{
    public class SubtractFunction : AbstractPairArithmeticFunction
    {
        public override string Description => "Subtracts two values from each other.";

        public override string Name => "Subtract";

        public override string[] Keywords => new[] { "Subtraction", "Minus", "-" };

        protected override string Verb => "subtract";

        protected override double Calculate(double first, double second) => first - second;
    }
}
