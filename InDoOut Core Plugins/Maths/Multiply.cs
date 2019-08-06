namespace InDoOut_Core_Plugins.Maths
{
    public class Multiply : GenericArithmetic
    {
        public override string Description => "Multiplies the first number with the second.";

        public override string Name => "Multiply";

        public override string[] Keywords => new[] { "Multipliacation", "Times", "*", "x", "×", ".", "⋅" };

        protected override double Calculate(double first, double second)
        {
            return first * second;
        }
    }
}
