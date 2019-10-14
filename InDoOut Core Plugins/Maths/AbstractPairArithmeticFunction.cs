using InDoOut_Core.Entities.Functions;

namespace InDoOut_Core_Plugins.Maths
{
    public abstract class AbstractPairArithmeticFunction : Function
    {
        private readonly IProperty<double> _firstNumber;
        private readonly IProperty<double> _secondNumber;

        private readonly IResult _result = new Result("Result value", "The result of the calculation", "0");

        private readonly IOutput _outputCalculated;
        private readonly IOutput _outputFailed;

        public override string Group => "Maths";

        public override IOutput TriggerOnFailure => _outputFailed;

        protected abstract string Verb { get; }

        public AbstractPairArithmeticFunction() : base()
        {
            _firstNumber = AddProperty(new Property<double>("First number", $"The first number to {Verb}", true, 0));
            _secondNumber = AddProperty(new Property<double>("Second number", $"The second number to {Verb}", true, 0));

            _ = AddResult(_result);

            _ = CreateInput("Calculate");

            _outputCalculated = CreateOutput(OutputType.Positive, "Calculated");
            _outputFailed = CreateOutput(OutputType.Negative, "Calculation failed");
        }

        protected override IOutput Started(IInput triggeredBy)
        {
            try
            {
                var success = _result.ValueFrom(Calculate(_firstNumber.FullValue, _secondNumber.FullValue));
                if (!success || double.IsNaN(_result.ValueAs<double>()))
                {
                    _ = _result.ValueFrom<double>(0);
                    return _outputFailed;
                }

                return _outputCalculated;
            }
            catch { }

            return _outputFailed;
        }

        protected abstract double Calculate(double first, double second);
    }
}
