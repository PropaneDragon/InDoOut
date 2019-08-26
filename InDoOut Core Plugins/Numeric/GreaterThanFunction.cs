using InDoOut_Core.Entities.Functions;

namespace InDoOut_Core_Plugins.Numeric
{
    public class GreaterThanFunction : Function
    {
        private readonly IOutput _less = null, _more = null;
        private readonly IProperty<double> _firstNumber = null, _secondNumber = null;

        public override string Description => "Does a greater than (>) comparison check between two numbers.";

        public override string Name => "Greater than";

        public override string Group => "Numeric";

        public override string[] Keywords => new[] { "greater", "than", ">", "gt", "over", "above", "bigger", "more", "numeric", "number", "check" };

        public GreaterThanFunction()
        {
            _ = CreateInput("Compare");

            _more = CreateOutput("First number is more", OutputType.Positive);
            _less = CreateOutput("First number not more", OutputType.Negative);

            _firstNumber = AddProperty(new Property<double>("First number", "The comparison number."));
            _secondNumber = AddProperty(new Property<double>("Second number", "The number to compare against."));
        }

        protected override IOutput Started(IInput triggeredBy)
        {
            return _firstNumber.FullValue > _secondNumber.FullValue ? _more : _less;
        }
    }
}
