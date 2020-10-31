using InDoOut_Core.Entities.Functions;

namespace InDoOut_Core_Plugins.Numeric
{
    public class LessThanFunction : Function
    {
        private readonly IOutput _less = null, _more = null;
        private readonly IProperty<double> _firstNumber = null, _secondNumber = null;

        public override string Description => "Does a less than (<) comparison check between two numbers.";

        public override string Name => "Less than";

        public override string Group => "Numeric";

        public override string[] Keywords => new[] { "less", "than", "<", "lt", "under", "below", "smaller", "numeric", "number", "check" };

        public LessThanFunction()
        {
            _ = CreateInput("Compare");

            _less = CreateOutput("First number is less", OutputType.Positive);
            _more = CreateOutput("First number not less", OutputType.Negative);

            _firstNumber = AddProperty(new Property<double>("First number", "The comparison number."));
            _secondNumber = AddProperty(new Property<double>("Second number", "The number to compare against."));
        }

        protected override IOutput Started(IInput triggeredBy) => _firstNumber.FullValue < _secondNumber.FullValue ? _less : _more;
    }
}
