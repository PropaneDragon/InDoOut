using InDoOut_Core.Entities.Functions;

namespace InDoOut_Core_Plugins.Numeric
{
    public class NumericCompareFunction : Function
    {
        private readonly IOutput _equal, _firstNumberSmaller, _firstNumberLarger;
        private readonly IProperty<double> _firstNumber = null, _secondNumber = null;

        public override string Description => "Compares two numbers together and outputs whether they're equal, smaller or bigger.";

        public override string Name => "Compare numbers";

        public override string Group => "Numeric";

        public override string[] Keywords => new[] { "compare", "bigger", "smaller", "larger", "greater", "less", "comparison" };

        public NumericCompareFunction()
        {
            _ = CreateInput("Compare");

            _firstNumberSmaller = CreateOutput("First number is less", OutputType.Neutral);
            _firstNumberLarger = CreateOutput("First number is larger", OutputType.Neutral);
            _equal = CreateOutput("Numbers are equal", OutputType.Neutral);

            _firstNumber = AddProperty(new Property<double>("First number", "The comparison number."));
            _secondNumber = AddProperty(new Property<double>("Second number", "The number to compare against."));
        }

        protected override IOutput Started(IInput triggeredBy) => _firstNumber.FullValue == _secondNumber.FullValue ? _equal : _firstNumber.FullValue > _secondNumber.FullValue ? _firstNumberLarger : _firstNumberSmaller;
    }
}
