using InDoOut_Core.Entities.Functions;

namespace InDoOut_Core_Plugins.Numeric
{
    public class EqualToFunction : Function
    {
        private IOutput _equal = null, _different = null;
        private IProperty<double> _firstNumber = null, _secondNumber = null;

        public override string Description => "Checks whether two numbers are the same as each other.";

        public override string Name => "Equal";

        public override string Group => "Numeric";

        public override string[] Keywords => new[] { "equal", "to", "=", "==", "===", "eq", "match", "exact", "same", "numeric", "number", "check" };

        public EqualToFunction()
        {
            _ = CreateInput("Compare");

            _equal = CreateOutput("Numbers are equal", OutputType.Positive);
            _different = CreateOutput("Numbers are different", OutputType.Negative);

            _firstNumber = AddProperty(new Property<double>("First number", "The first number to check."));
            _secondNumber = AddProperty(new Property<double>("Second number", "The second number to check."));
        }

        protected override IOutput Started(IInput triggeredBy)
        {
            return _firstNumber.FullValue == _secondNumber.FullValue ? _equal : _different;
        }
    }
}
