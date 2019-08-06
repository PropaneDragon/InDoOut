namespace InDoOut_Core_Plugins.Maths
{
    public class DivideFunction : AbstractArithmeticFunction
    {
        public override string Description => "Divides the first number by the second.";

        public override string Name => "Divide";

        public override string[] Keywords => new[] { "Division", "÷", ":", "/" };

        protected override double Calculate(double first, double second)
        {
            return first / second;
        }
    }
}
