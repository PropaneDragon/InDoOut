using InDoOut_Core.Entities.Functions;

namespace InDoOut_Core_Plugins.Maths
{
    public class Add : Function
    {
        private Property<int> _firstNumber = new Property<int>("First number", "The first number to add", true, 0);
        private Property<int> _secondNumber = new Property<int>("Second number", "The second number to add", true, 0);

        private Result _additionResult = new Result("Result", "The result of the addition", "0");

        private IOutput _outputCalculated;
        private IOutput _outputFailed;

        public override string Name => "Add";

        public override string Description => "Adds two values together";

        public override string Group => "Maths";

        public override string[] Keywords => new[] { "Addition", "Maths", "Plus" };

        public Add() : base()
        {
            _ = AddProperty(_firstNumber);
            _ = AddProperty(_secondNumber);

            _ = AddResult(_additionResult);

            _ = CreateInput("Calculate");

            _outputCalculated = CreateOutput("Calculated");
            _outputFailed = CreateOutput(OutputType.Negative, "Failed");
        }

        protected override IOutput Started(IInput triggeredBy)
        {
            return _additionResult.ValueFrom(_firstNumber.Value + _secondNumber.Value) ? _outputCalculated : _outputFailed;
        }
    }
}
