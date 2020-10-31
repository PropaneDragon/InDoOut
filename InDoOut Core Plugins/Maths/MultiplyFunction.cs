namespace InDoOut_Core_Plugins.Maths
{
    public class MultiplyFunction : AbstractPairArithmeticFunction
    {
        public override string Description => "Multiplies the first number with the second.";

        public override string Name => "Multiply";

        public override string[] Keywords => new[] { "Multipliacation", "Times", "*", "x", "×", ".", "⋅" };

        protected override string Verb => "multiply";

        protected override double Calculate(double first, double second) => first * second;
    }
}
