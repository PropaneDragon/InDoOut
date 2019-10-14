using InDoOut_Core.Entities.Functions;

namespace InDoOut_Core_Plugins.Maths
{
    public abstract class AbstractSingularArithmeticFunction : Function
    {
        private readonly IProperty<double> _number;

        private readonly IResult _result = new Result("Result value", "The result of the calculation", "0");

        private readonly IOutput _outputCalculated;
        private readonly IOutput _outputFailed;

        public override string Group => "Maths";

        public override IOutput TriggerOnFailure => _outputFailed;

        protected abstract string Verb { get; }

        public AbstractSingularArithmeticFunction() : base()
        {
            _number = AddProperty(new Property<double>("Number", $"The number to {Verb}", true, 0));

            _ = AddResult(_result);

            _ = CreateInput("Calculate");

            _outputCalculated = CreateOutput(OutputType.Positive, "Calculated");
            _outputFailed = CreateOutput(OutputType.Negative, "Calculation failed");
        }

        protected override IOutput Started(IInput triggeredBy)
        {
            try
            {
                var success = _result.ValueFrom(Calculate(_number.FullValue));
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

        protected abstract double Calculate(double number);
    }
}
