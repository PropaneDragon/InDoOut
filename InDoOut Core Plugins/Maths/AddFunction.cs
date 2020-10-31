namespace InDoOut_Core_Plugins.Maths
{
    public class AddFunction : AbstractPairArithmeticFunction
    {
        public override string Description => "Adds two values together.";

        public override string Name => "Add";

        public override string[] Keywords => new[] { "Addition", "Plus", "+" };

        protected override string Verb => "add";

        protected override double Calculate(double first, double second) => first + second;
    }
}
