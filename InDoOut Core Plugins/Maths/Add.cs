namespace InDoOut_Core_Plugins.Maths
{
    public class Add : GenericArithmetic
    {
        public override string Description => "Adds two values together.";

        public override string Name => "Add";

        public override string[] Keywords => new[] { "Addition", "Plus", "+" };

        protected override double Calculate(double first, double second)
        {
            return first + second;
        }
    }
}
