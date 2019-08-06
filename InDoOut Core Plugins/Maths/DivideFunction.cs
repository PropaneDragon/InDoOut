namespace InDoOut_Core_Plugins.Maths
{
    public class DivideFunction : AbstractPairArithmeticFunction
    {
        public override string Description => "Divides the first number by the second.";

        public override string Name => "Divide";

        public override string[] Keywords => new[] { "Division", "÷", ":", "/" };

        protected override string Verb => "divide";

        protected override double Calculate(double first, double second)
        {
            return first / second;
        }
    }
}
